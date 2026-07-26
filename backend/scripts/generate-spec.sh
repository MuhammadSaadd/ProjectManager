#!/usr/bin/env bash
set -euo pipefail

API_PROJECT="$(cd "$(dirname "$0")/../src/Api" && pwd)"
SCHEMAS_DIR="$(cd "$(dirname "$0")/../../schemas" && pwd)"
PORT=5199

cleanup() {
  if [ -n "${API_PID:-}" ]; then
    kill "$API_PID" 2>/dev/null || true
    wait "$API_PID" 2>/dev/null || true
  fi
}
trap cleanup EXIT

dotnet run --project "$API_PROJECT" --urls "http://localhost:$PORT" --no-build &
API_PID=$!

for i in $(seq 1 10); do
  if curl -sf "http://localhost:$PORT/openapi/v1.json" > /dev/null 2>&1; then
    break
  fi
  sleep 1
done

curl -sf "http://localhost:$PORT/openapi/v1.json" -o "$SCHEMAS_DIR/api.json"
echo "Generated $SCHEMAS_DIR/api.json"
