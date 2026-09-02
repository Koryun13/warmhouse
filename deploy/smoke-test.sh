#!/usr/bin/env bash
# End-to-end check of the ecosystem through the API gateway (port 8000).
#
# Full path: user -> house -> device -> telemetry -> threshold -> scenario
# -> notification.
#
# Every endpoint except registration and sign-in requires a bearer token, and
# house-scoped resources require a token that carries that house.
#
# Запуск:  bash deploy/smoke-test.sh
set -euo pipefail

GW="${GATEWAY_URL:-http://localhost:8000}"
RUN_ID="$(date +%s)"
EMAIL="resident-$RUN_ID@warmhouse.example"
PASSWORD="warmhouse-2026"
TMP="$(mktemp -d)"
trap 'rm -rf "$TMP"' EXIT

say()  { printf '\n\033[1m== %s\033[0m\n' "$1"; }

TOKEN=""
AUTH=()

# Заголовок собирается массивом: строка с пробелами, подставленная без
# кавычек, распалась бы на несколько аргументов curl.
set_auth() {
    if [ -n "$TOKEN" ]; then
        AUTH=(-H "Authorization: Bearer $TOKEN")
    else
        AUTH=()
    fi
}

# Тело запроса всегда передаётся файлом: так кириллица и знак градуса
# доходят до сервиса в UTF-8 независимо от кодировки терминала.
post() {
    local url="$1" file="$2"
    set_auth
    curl -sS -X POST "$url" \
        -H 'Content-Type: application/json' \
        ${AUTH[@]+"${AUTH[@]}"} \
        --data-binary "@$file"
}

get() {
    set_auth
    curl -sS "$1" ${AUTH[@]+"${AUTH[@]}"}
}

# Первый "id" верхнего уровня в ответе.
id_of() { grep -o '"id":"[^"]*"' | head -1 | cut -d'"' -f4; }

# Токен выпускается заново после выдачи доступа к дому: claim с домом
# попадает в токен в момент выпуска.
issue_token() {
    cat > "$TMP/token.json" <<EOF
{"email":"$EMAIL","password":"$PASSWORD"}
EOF
    local response
    response=$(curl -sS -X POST "$GW/api/v1/auth/token" \
        -H 'Content-Type: application/json' --data-binary "@$TMP/token.json")
    TOKEN=$(printf '%s' "$response" | grep -o '"access_token":"[^"]*"' | cut -d'"' -f4)

    if [ -z "$TOKEN" ]; then
        printf '\033[1;31mНе удалось получить токен: %s\033[0m\n' "$response" >&2
        exit 1
    fi
}

say "1. Регистрация пользователя"
cat > "$TMP/user.json" <<EOF
{"email":"$EMAIL","display_name":"Житель","password":"$PASSWORD"}
EOF
USER_JSON=$(post "$GW/api/v1/users" "$TMP/user.json"); echo "$USER_JSON"
USER_ID=$(printf '%s' "$USER_JSON" | id_of)

say "2. Получение токена"
issue_token
echo "Токен получен, домов в нём пока нет."

say "3. Создание дома (владелец — предъявитель токена)"
cat > "$TMP/house.json" <<EOF
{"name":"Дом в Алматы","address":"ул. Абая, 1"}
EOF
HOUSE_JSON=$(post "$GW/api/v1/houses" "$TMP/house.json"); echo "$HOUSE_JSON"
HOUSE_ID=$(printf '%s' "$HOUSE_JSON" | id_of)

say "4. Перевыпуск токена: в нём появился доступ к дому"
issue_token

say "5. Каталог типов устройств"
get "$GW/api/v1/device-types" | head -c 500; echo

say "6. Подключение термостата (самообслуживание)"
cat > "$TMP/device.json" <<EOF
{"house_id":"$HOUSE_ID","device_type_code":"thermostat.warmhouse.v1",
 "serial_number":"TH-$RUN_ID","name":"Термостат в гостиной","location":"Living Room"}
EOF
DEVICE_JSON=$(post "$GW/api/v1/devices" "$TMP/device.json"); echo "$DEVICE_JSON"
DEVICE_ID=$(printf '%s' "$DEVICE_JSON" | id_of)

say "7. Зона отопления заведена сервисом heating по событию DeviceRegistered"
sleep 3
get "$GW/api/v1/heating/zones?houseId=$HOUSE_ID"; echo

say "8. Правило контроля порога: температура ниже 18 градусов"
cat > "$TMP/rule.json" <<EOF
{"house_id":"$HOUSE_ID","device_id":"$DEVICE_ID","metric":"temperature","comparison":"lt","threshold":18.0}
EOF
post "$GW/api/v1/threshold-rules" "$TMP/rule.json"; echo

say "9. Сценарий: при срабатывании порога уведомить владельца"
cat > "$TMP/scenario.json" <<EOF
{"house_id":"$HOUSE_ID","name":"Холодно в гостиной","trigger_kind":"telemetry_threshold",
 "trigger_metric":"temperature","trigger_device_id":"$DEVICE_ID",
 "actions":[{"kind":"notify","recipient_id":"$USER_ID","channel":"push",
             "subject":"В гостиной холодно","body":"Температура опустилась ниже 18 °C"}]}
EOF
post "$GW/api/v1/scenarios" "$TMP/scenario.json"; echo

say "10. Телеметрия: 15.5 °C — ниже порога"
cat > "$TMP/telemetry.json" <<EOF
{"device_id":"$DEVICE_ID","house_id":"$HOUSE_ID","metric":"temperature","value":15.5,"unit":"°C"}
EOF
post "$GW/api/v1/telemetry" "$TMP/telemetry.json"
sleep 4

say "11. Измерение сохранено сервисом телеметрии"
get "$GW/api/v1/telemetry?houseId=$HOUSE_ID&deviceId=$DEVICE_ID&metric=temperature"; echo

say "12. Уведомление создано сценарием"
get "$GW/api/v1/notifications"; echo

say "13. Без токена доступ закрыт (ожидается 401)"
curl -sS -o /dev/null -w 'GET /api/v1/notifications -> %{http_code}\n' \
    "$GW/api/v1/notifications"

say "14. Чужой дом недоступен (ожидается 403)"
curl -sS -o /dev/null -w 'GET /api/v1/heating/zones -> %{http_code}\n' \
    -H "Authorization: Bearer $TOKEN" \
    "$GW/api/v1/heating/zones?houseId=00000000-0000-0000-0000-000000000001"

printf '\n\033[1;32mСквозная проверка завершена.\033[0m\n'
