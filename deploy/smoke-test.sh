#!/usr/bin/env bash
# End-to-end check of the ecosystem through the API gateway (port 8000).
#
# Full path: user -> house -> device -> telemetry -> threshold -> scenario
# -> notification.
#
# Запуск:  bash deploy/smoke-test.sh
set -euo pipefail

GW="${GATEWAY_URL:-http://localhost:8000}"
RUN_ID="$(date +%s)"
TMP="$(mktemp -d)"
trap 'rm -rf "$TMP"' EXIT

say()  { printf '\n\033[1m== %s\033[0m\n' "$1"; }

# Тело запроса всегда передаётся файлом: так кириллица и знак градуса
# доходят до сервиса в UTF-8 независимо от кодировки терминала.
post() {
    local url="$1" file="$2"
    curl -sS -X POST "$url" -H 'Content-Type: application/json' --data-binary "@$file"
}

# Первый "id" верхнего уровня в ответе.
id_of() { grep -o '"id":"[^"]*"' | head -1 | cut -d'"' -f4; }

say "1. Регистрация пользователя"
cat > "$TMP/user.json" <<EOF
{"email":"resident-$RUN_ID@warmhouse.kz","display_name":"Житель","password":"warmhouse-2026"}
EOF
USER_JSON=$(post "$GW/api/v1/users" "$TMP/user.json"); echo "$USER_JSON"
USER_ID=$(printf '%s' "$USER_JSON" | id_of)

say "2. Получение токена"
cat > "$TMP/token.json" <<EOF
{"email":"resident-$RUN_ID@warmhouse.kz","password":"warmhouse-2026"}
EOF
post "$GW/api/v1/auth/token" "$TMP/token.json"; echo

say "3. Создание дома"
cat > "$TMP/house.json" <<EOF
{"owner_id":"$USER_ID","name":"Дом в Алматы","address":"ул. Абая, 1"}
EOF
HOUSE_JSON=$(post "$GW/api/v1/houses" "$TMP/house.json"); echo "$HOUSE_JSON"
HOUSE_ID=$(printf '%s' "$HOUSE_JSON" | id_of)

say "4. Каталог типов устройств"
curl -sS "$GW/api/v1/device-types" | head -c 500; echo

say "5. Подключение термостата (самообслуживание)"
cat > "$TMP/device.json" <<EOF
{"house_id":"$HOUSE_ID","owner_id":"$USER_ID","device_type_code":"thermostat.warmhouse.v1",
 "serial_number":"TH-$RUN_ID","name":"Термостат в гостиной","location":"Living Room"}
EOF
DEVICE_JSON=$(post "$GW/api/v1/devices" "$TMP/device.json"); echo "$DEVICE_JSON"
DEVICE_ID=$(printf '%s' "$DEVICE_JSON" | id_of)

say "6. Зона отопления заведена сервисом heating по событию DeviceRegistered"
sleep 2
curl -sS "$GW/api/v1/heating/zones?houseId=$HOUSE_ID"; echo

say "7. Правило контроля порога: температура ниже 18 градусов"
cat > "$TMP/rule.json" <<EOF
{"house_id":"$HOUSE_ID","device_id":"$DEVICE_ID","metric":"temperature","comparison":"lt","threshold":18.0}
EOF
post "$GW/api/v1/threshold-rules" "$TMP/rule.json"; echo

say "8. Сценарий: при срабатывании порога уведомить владельца"
cat > "$TMP/scenario.json" <<EOF
{"house_id":"$HOUSE_ID","name":"Холодно в гостиной","trigger_kind":"telemetry_threshold",
 "trigger_metric":"temperature","trigger_device_id":"$DEVICE_ID",
 "actions":[{"kind":"notify","recipient_id":"$USER_ID","channel":"push",
             "subject":"В гостиной холодно","body":"Температура опустилась ниже 18 °C"}]}
EOF
post "$GW/api/v1/scenarios" "$TMP/scenario.json"; echo

say "9. Телеметрия: 15.5 °C — ниже порога"
cat > "$TMP/telemetry.json" <<EOF
{"device_id":"$DEVICE_ID","house_id":"$HOUSE_ID","metric":"temperature","value":15.5,"unit":"°C"}
EOF
post "$GW/api/v1/telemetry" "$TMP/telemetry.json"
sleep 3

say "10. Измерение сохранено сервисом телеметрии"
curl -sS "$GW/api/v1/telemetry?deviceId=$DEVICE_ID&metric=temperature"; echo

say "11. Уведомление создано сценарием"
curl -sS "$GW/api/v1/notifications?recipientId=$USER_ID"; echo

printf '\n\033[1;32mСквозная проверка завершена.\033[0m\n'
