#!/usr/bin/env bash
# End-to-end test of the ecosystem through the API gateway (port 8000).
#
# Full path: user -> house -> device -> telemetry -> threshold -> scenario
# -> notification, plus the authorisation rules that guard it.
#
# Every step asserts on the status code and on the payload, and the script exits
# non-zero if any assertion fails, so it is usable as a gate in CI rather than
# something a human has to read. The responses are still printed as they arrive:
# the assertions decide the outcome, the output shows the real data behind it.
#
# Запуск:  bash deploy/smoke-test.sh
#
# Переменные: GATEWAY_URL, READY_TIMEOUT, EVENT_TIMEOUT.
#
# set -e is deliberately absent: a failing assertion has to be recorded and the
# run continued, otherwise the first failure hides every later one.
set -uo pipefail

GW="${GATEWAY_URL:-http://localhost:8000}"
READY_TIMEOUT="${READY_TIMEOUT:-120}"
EVENT_TIMEOUT="${EVENT_TIMEOUT:-45}"

RUN_ID="$(date +%s)-$$"
EMAIL="resident-$RUN_ID@warmhouse.example"
PASSWORD="warmhouse-2026"
SERIAL="TH-$RUN_ID"
FOREIGN_HOUSE="00000000-0000-0000-0000-000000000001"

TMP="$(mktemp -d)"
trap 'rm -rf "$TMP"' EXIT

BOLD='\033[1m'; RED='\033[1;31m'; GREEN='\033[1;32m'; DIM='\033[2m'; OFF='\033[0m'

PASSED=0
FAILED=0
FAILED_NAMES=()

TOKEN=""
STATUS=""
BODY=""

section() { printf '\n%b== %s%b\n' "$BOLD" "$1" "$OFF"; }

pass() {
    PASSED=$((PASSED + 1))
    printf '  %bok%b   %s\n' "$GREEN" "$OFF" "$1"
}

fail() {
    FAILED=$((FAILED + 1))
    FAILED_NAMES+=("$1")
    printf '  %bFAIL%b %s\n' "$RED" "$OFF" "$1"
    if [ -n "${2:-}" ]; then
        printf '       %b%s%b\n' "$DIM" "$2" "$OFF"
    fi
    return 0
}

# Prints what the service actually returned. The assertions decide whether the
# run passes; this is what makes the run also readable as a walkthrough.
show() {
    if [ -n "$BODY" ]; then
        printf '       %b%s%b\n' "$DIM" "$BODY" "$OFF"
    fi
}

# Performs a request and publishes the outcome in STATUS and BODY. The bearer
# token is attached whenever one is held; pass "anon" as the first argument to
# send the request without it.
request() {
    local anon=0
    if [ "$1" = "anon" ]; then anon=1; shift; fi

    local method="$1" url="$2" file="${3:-}"
    local out="$TMP/response"
    local args=(-sS --max-time 30 -o "$out" -w '%{http_code}' -X "$method" "$url")

    if [ "$anon" -eq 0 ] && [ -n "$TOKEN" ]; then
        args+=(-H "Authorization: Bearer $TOKEN")
    fi

    if [ -n "$file" ]; then
        args+=(-H 'Content-Type: application/json' --data-binary "@$file")
    fi

    STATUS="$(curl "${args[@]}" 2>/dev/null)" || STATUS="000"
    BODY="$(cat "$out" 2>/dev/null)"
    rm -f "$out"
}

assert_status() {
    local expected="$1" name="$2"
    if [ "$STATUS" = "$expected" ]; then
        pass "$name"
    else
        fail "$name" "ожидался HTTP $expected, получен $STATUS: $(printf '%s' "$BODY" | head -c 300)"
    fi
}

assert_body_contains() {
    local needle="$1" name="$2"
    if printf '%s' "$BODY" | grep -qF -- "$needle"; then
        pass "$name"
    else
        fail "$name" "в ответе нет '$needle': $(printf '%s' "$BODY" | head -c 300)"
    fi
}

# The problem-details payload carries the domain error code, so an assertion can
# pin the exact failure instead of only its HTTP class.
assert_error_code() {
    local expected="$1" name="$2"
    assert_body_contains "\"code\":\"$expected\"" "$name"
}

# First top-level "id" of the response.
id_of() { printf '%s' "$BODY" | grep -o '"id":"[^"]*"' | head -1 | cut -d'"' -f4; }

token_of() { printf '%s' "$BODY" | grep -o '"access_token":"[^"]*"' | cut -d'"' -f4; }

abort() {
    printf '\n%b%s%b\n' "$RED" "$1" "$OFF" >&2
    exit 1
}

# Polls a predicate until it holds or the deadline passes. The asynchronous
# steps depend on broker delivery, so a fixed sleep is either flaky or slow.
retry_until() {
    local seconds="$1" name="$2"; shift 2
    local deadline=$(( $(date +%s) + seconds ))

    while :; do
        if "$@"; then
            pass "$name"
            show
            return 0
        fi
        if [ "$(date +%s)" -ge "$deadline" ]; then
            fail "$name" "не выполнено за ${seconds}s; последний ответ HTTP $STATUS: $(printf '%s' "$BODY" | head -c 300)"
            return 1
        fi
        sleep 2
    done
}

# --------------------------------------------------------------------------
section "0. Готовность шлюза"

gateway_ready() {
    request anon GET "$GW/health"
    [ "$STATUS" = "200" ]
}
retry_until "$READY_TIMEOUT" "шлюз отвечает на /health" gateway_ready \
    || abort "Шлюз недоступен на $GW — стек не поднят."

# --------------------------------------------------------------------------
section "1. Политика запрета по умолчанию"

request anon GET "$GW/api/v1/notifications"
assert_status 401 "GET /notifications без токена -> 401"

request anon GET "$GW/api/v1/houses"
assert_status 401 "GET /houses без токена -> 401"

request anon GET "$GW/api/v1/devices?houseId=$FOREIGN_HOUSE"
assert_status 401 "GET /devices без токена -> 401"

TOKEN="not-a-real-token"
request GET "$GW/api/v1/houses"
assert_status 401 "GET /houses с мусорным токеном -> 401"
TOKEN=""

# --------------------------------------------------------------------------
section "2. Регистрация пользователя и выпуск токена"

cat > "$TMP/user.json" <<JSON
{"email":"$EMAIL","display_name":"Житель","password":"$PASSWORD"}
JSON
request anon POST "$GW/api/v1/users" "$TMP/user.json"
assert_status 201 "POST /users -> 201"
show
assert_body_contains "$EMAIL" "ответ содержит зарегистрированный email"
USER_ID="$(id_of)"

if [ -n "$USER_ID" ]; then
    pass "в ответе есть идентификатор пользователя"
else
    fail "в ответе есть идентификатор пользователя" "id не найден"
fi

request anon POST "$GW/api/v1/users" "$TMP/user.json"
assert_status 409 "повторная регистрация -> 409"
assert_error_code "user.email_taken" "код ошибки user.email_taken"

cat > "$TMP/weak.json" <<JSON
{"email":"weak-$RUN_ID@warmhouse.example","display_name":"Слабый пароль","password":"123"}
JSON
request anon POST "$GW/api/v1/users" "$TMP/weak.json"
assert_status 400 "слишком короткий пароль -> 400"
assert_error_code "user.invalid_registration" "код ошибки user.invalid_registration"

cat > "$TMP/badpass.json" <<JSON
{"email":"$EMAIL","password":"wrong-password"}
JSON
request anon POST "$GW/api/v1/auth/token" "$TMP/badpass.json"
assert_status 401 "неверный пароль -> 401"
assert_error_code "auth.invalid_credentials" "код ошибки auth.invalid_credentials"

cat > "$TMP/unknown.json" <<JSON
{"email":"nobody-$RUN_ID@warmhouse.example","password":"$PASSWORD"}
JSON
request anon POST "$GW/api/v1/auth/token" "$TMP/unknown.json"
assert_status 401 "неизвестный email -> 401 (неотличимо от неверного пароля)"

cat > "$TMP/token.json" <<JSON
{"email":"$EMAIL","password":"$PASSWORD"}
JSON
request anon POST "$GW/api/v1/auth/token" "$TMP/token.json"
assert_status 200 "POST /auth/token -> 200"
assert_body_contains '"access_token"' "выдан access_token"

TOKEN="$(token_of)"
[ -n "$TOKEN" ] || abort "Без токена продолжать нечем."
TOKEN_WITHOUT_HOUSE="$TOKEN"

# --------------------------------------------------------------------------
section "3. Дом и claim, зафиксированный в момент выпуска"

cat > "$TMP/house.json" <<JSON
{"name":"Дом в Алматы","address":"ул. Абая, 1"}
JSON
request POST "$GW/api/v1/houses" "$TMP/house.json"
assert_status 201 "POST /houses -> 201"
show
HOUSE_ID="$(id_of)"
[ -n "$HOUSE_ID" ] || abort "Дом не создан, дальнейшие проверки невозможны."

# The claim is written when the token is issued, so the token obtained before
# the house existed must still be refused for it.
request GET "$GW/api/v1/heating/zones?houseId=$HOUSE_ID"
assert_status 403 "старый токен не даёт доступа к новому дому -> 403"
assert_error_code "access.house_forbidden" "код ошибки access.house_forbidden"

request anon POST "$GW/api/v1/auth/token" "$TMP/token.json"
assert_status 200 "перевыпуск токена -> 200"
TOKEN="$(token_of)"
[ -n "$TOKEN" ] || abort "Перевыпуск токена не удался."

if [ "$TOKEN" != "$TOKEN_WITHOUT_HOUSE" ]; then
    pass "перевыпущенный токен отличается от прежнего"
else
    fail "перевыпущенный токен отличается от прежнего" "выдан тот же самый токен"
fi

request GET "$GW/api/v1/houses"
assert_status 200 "GET /houses -> 200"
show
assert_body_contains "$HOUSE_ID" "список домов содержит созданный дом"

request GET "$GW/api/v1/heating/zones?houseId=$HOUSE_ID"
assert_status 200 "перевыпущенный токен открывает дом -> 200"

# --------------------------------------------------------------------------
section "4. Каталог типов и подключение термостата"

request GET "$GW/api/v1/device-types"
assert_status 200 "GET /device-types -> 200"
printf '       %bкоды: %s%b\n' "$DIM" \
    "$(printf '%s' "$BODY" | grep -o '"code":"[^"]*"' | cut -d'"' -f4 | paste -sd',' - | sed 's/,/, /g')" "$OFF"
assert_body_contains "thermostat.warmhouse.v1" "каталог содержит термостат из сидов"

cat > "$TMP/unknown-type.json" <<JSON
{"house_id":"$HOUSE_ID","device_type_code":"no.such.device.v1",
 "serial_number":"UNKNOWN-$RUN_ID","name":"Неизвестный прибор"}
JSON
request POST "$GW/api/v1/devices" "$TMP/unknown-type.json"
assert_status 422 "неизвестный тип устройства -> 422"
assert_error_code "device_type.not_found" "код ошибки device_type.not_found"

cat > "$TMP/foreign-device.json" <<JSON
{"house_id":"$FOREIGN_HOUSE","device_type_code":"thermostat.warmhouse.v1",
 "serial_number":"FOREIGN-$RUN_ID","name":"Термостат в чужом доме"}
JSON
request POST "$GW/api/v1/devices" "$TMP/foreign-device.json"
assert_status 403 "подключение в чужой дом -> 403"
assert_error_code "access.house_forbidden" "код ошибки access.house_forbidden"

cat > "$TMP/device.json" <<JSON
{"house_id":"$HOUSE_ID","device_type_code":"thermostat.warmhouse.v1",
 "serial_number":"$SERIAL","name":"Термостат в гостиной","location":"Living Room"}
JSON
request POST "$GW/api/v1/devices" "$TMP/device.json"
assert_status 201 "POST /devices -> 201"
show
assert_body_contains "$SERIAL" "ответ содержит серийный номер"
DEVICE_ID="$(id_of)"
[ -n "$DEVICE_ID" ] || abort "Устройство не создано, дальнейшие проверки невозможны."

request POST "$GW/api/v1/devices" "$TMP/device.json"
assert_status 409 "повторный серийный номер -> 409"
assert_error_code "device.serial_taken" "код ошибки device.serial_taken"

request GET "$GW/api/v1/devices/$DEVICE_ID"
assert_status 200 "GET /devices/{id} -> 200"
assert_body_contains "$DEVICE_ID" "ответ описывает запрошенное устройство"

request GET "$GW/api/v1/devices/00000000-0000-0000-0000-0000000000ff"
assert_status 404 "несуществующее устройство -> 404"

# --------------------------------------------------------------------------
section "5. Зона отопления заведена по событию DeviceRegistered"

zone_projected() {
    request GET "$GW/api/v1/heating/zones?houseId=$HOUSE_ID"
    [ "$STATUS" = "200" ] && printf '%s' "$BODY" | grep -qF "$DEVICE_ID"
}
retry_until "$EVENT_TIMEOUT" "heating завёл зону по событию DeviceRegistered" zone_projected

# --------------------------------------------------------------------------
section "6. Порог, сценарий и телеметрия"

cat > "$TMP/rule.json" <<JSON
{"house_id":"$HOUSE_ID","device_id":"$DEVICE_ID","metric":"temperature",
 "comparison":"lt","threshold":18.0}
JSON
request POST "$GW/api/v1/threshold-rules" "$TMP/rule.json"
assert_status 201 "POST /threshold-rules -> 201"
show

cat > "$TMP/foreign-rule.json" <<JSON
{"house_id":"$FOREIGN_HOUSE","metric":"temperature","comparison":"lt","threshold":18.0}
JSON
request POST "$GW/api/v1/threshold-rules" "$TMP/foreign-rule.json"
assert_status 403 "правило в чужом доме -> 403"

cat > "$TMP/scenario.json" <<JSON
{"house_id":"$HOUSE_ID","name":"Холодно в гостиной","trigger_kind":"telemetry_threshold",
 "trigger_metric":"temperature","trigger_device_id":"$DEVICE_ID",
 "actions":[{"kind":"notify","recipient_id":"$USER_ID","channel":"push",
             "subject":"В гостиной холодно $RUN_ID","body":"Температура опустилась ниже 18 °C"}]}
JSON
request POST "$GW/api/v1/scenarios" "$TMP/scenario.json"
assert_status 201 "POST /scenarios -> 201"
show
assert_body_contains "telemetry_threshold" "сценарий сохранён со своим триггером"

cat > "$TMP/telemetry.json" <<JSON
{"device_id":"$DEVICE_ID","house_id":"$HOUSE_ID","metric":"temperature",
 "value":15.5,"unit":"°C"}
JSON
request POST "$GW/api/v1/telemetry" "$TMP/telemetry.json"
assert_status 202 "POST /telemetry (15.5 °C — ниже порога) -> 202"

measurement_stored() {
    request GET "$GW/api/v1/telemetry?houseId=$HOUSE_ID&deviceId=$DEVICE_ID&metric=temperature"
    [ "$STATUS" = "200" ] && printf '%s' "$BODY" | grep -qF '15.5'
}
retry_until "$EVENT_TIMEOUT" "измерение сохранено сервисом телеметрии" measurement_stored

# The whole chain: telemetry -> threshold breach -> scenario -> notification.
notification_delivered() {
    request GET "$GW/api/v1/notifications"
    [ "$STATUS" = "200" ] && printf '%s' "$BODY" | grep -qF "В гостиной холодно $RUN_ID"
}
retry_until "$EVENT_TIMEOUT" "сценарий создал уведомление по пробитию порога" notification_delivered

# --------------------------------------------------------------------------
section "7. Изоляция чужого дома"

request GET "$GW/api/v1/heating/zones?houseId=$FOREIGN_HOUSE"
assert_status 403 "GET /heating/zones чужого дома -> 403"
assert_error_code "access.house_forbidden" "код ошибки access.house_forbidden"

request GET "$GW/api/v1/telemetry?houseId=$FOREIGN_HOUSE&deviceId=$DEVICE_ID&metric=temperature"
assert_status 403 "GET /telemetry чужого дома -> 403"

request GET "$GW/api/v1/scenarios?houseId=$FOREIGN_HOUSE"
assert_status 403 "GET /scenarios чужого дома -> 403"

# --------------------------------------------------------------------------
TOTAL=$((PASSED + FAILED))
printf '\n%b--------------------------------------------------%b\n' "$BOLD" "$OFF"

if [ "$FAILED" -eq 0 ]; then
    printf '%bВсе проверки пройдены: %d из %d.%b\n' "$GREEN" "$PASSED" "$TOTAL" "$OFF"
    exit 0
fi

printf '%bПровалено %d из %d:%b\n' "$RED" "$FAILED" "$TOTAL" "$OFF"
for name in "${FAILED_NAMES[@]}"; do
    printf '  - %s\n' "$name"
done
exit 1
