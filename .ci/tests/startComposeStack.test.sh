#!/bin/bash
set -euo pipefail

script_dir=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)
test_dir=$(mktemp -d)
compose_args=(-p "backbone-startup-test-$$" -f "$test_dir/compose.yml")
export STARTUP_TEST_STATE="$test_dir/state"
export FAILED_DATABASE_STARTS=0
export SEED_EXIT_CODE=0

cleanup() {
    docker compose "${compose_args[@]}" down --timeout 1
    rm -r -- "$test_dir"
}
trap cleanup EXIT

cat > "$test_dir/compose.yml" <<'YAML'
services:
  database:
    image: alpine:3.22
    environment:
      FAILED_DATABASE_STARTS: ${FAILED_DATABASE_STARTS}
    volumes:
      - ${STARTUP_TEST_STATE}:/state
    command:
      - sh
      - -c
      - |
        attempt=$$(cat /state/attempts 2>/dev/null || echo 0)
        attempt=$$((attempt + 1))
        echo "$$attempt" > /state/attempts
        if [ "$$attempt" -le "$$FAILED_DATABASE_STARTS" ]; then
          echo "Simulating SQLPAL crash with delayed process exit"
          sleep 5
          exit 1
        fi
        touch /tmp/ready
        exec sleep 300
    healthcheck:
      test: ["CMD", "test", "-f", "/tmp/ready"]
      interval: 1s
      timeout: 1s
      retries: 1
  seed-database:
    image: alpine:3.22
    environment:
      SEED_EXIT_CODE: ${SEED_EXIT_CODE}
    volumes:
      - ${STARTUP_TEST_STATE}:/state
    depends_on:
      database:
        condition: service_healthy
    command: ["sh", "-c", 'touch /state/seeded; exit "$$SEED_EXIT_CODE"']
  database-migrator:
    image: alpine:3.22
    depends_on:
      seed-database:
        condition: service_completed_successfully
    command: ["sleep", "300"]
YAML

run_case() {
    local name=$1 expected_status=$2 expected_attempts=$3 expected_seed=$4
    export FAILED_DATABASE_STARTS=$5 SEED_EXIT_CODE=$6
    export STARTUP_TEST_STATE="$test_dir/$name"
    mkdir -p "$STARTUP_TEST_STATE"

    local status=0
    "$script_dir/../startComposeStack.sh" "${compose_args[@]}" > "$test_dir/$name.log" 2>&1 || status=$?
    cat "$test_dir/$name.log"
    test "$status" -eq "$expected_status"
    test "$(cat "$STARTUP_TEST_STATE/attempts")" -eq "$expected_attempts"
    if [ "$expected_seed" = yes ]; then
        test -f "$STARTUP_TEST_STATE/seeded"
    else
        test ! -f "$STARTUP_TEST_STATE/seeded"
    fi
    docker compose "${compose_args[@]}" down --timeout 1
    echo "PASS: $name"
}

run_case healthy 0 1 yes 0 0
run_case startup-crash 0 2 yes 1 0
run_case last-attempt 0 3 yes 2 0
run_case permanent-startup-failure 1 3 no 3 0
run_case seed-failure 1 1 yes 0 1
