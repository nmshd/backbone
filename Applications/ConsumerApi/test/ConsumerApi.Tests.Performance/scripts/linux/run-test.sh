#!/bin/bash

# Default values
scenario=""
vus="1"
duration="10s"

# Parse arguments
while [[ $# -gt 0 ]]; do
    case "$1" in
        -s|--scenario)
            scenario="$2"
            shift 2
            ;;
        -v|--vus)
            vus="$2"
            shift 2
            ;;
        -d|--duration)
            duration="$2"
            shift 2
            ;;
        *)
            # Store remaining args for k6
            k6Arguments+=("$1")
            shift
            ;;
    esac
done

# Validate scenario is provided
if [[ -z "$scenario" ]]; then
    echo "Error: scenario parameter is required" >&2
    exit 1
fi

# Stop the script when a command fails
set -e

t=$(date +%Y%m%d-%H%M%S)
testFile="./dist/${scenario}.test.js"
outputFile="k6-outputs/${t}-${scenario}.csv"
resultAnalyzerFolder="./tools/result-analyzer"

npx webpack

cd tools/result-analyzer
npm install
cd ../..

mkdir -p k6-outputs

k6 run \
    --tag testid=$t \
    --out "csv=$outputFile" --out opentelemetry \
    --env K6_WEB_DASHBOARD_EXPORT=html-report.html --env K6_WEB_DASHBOARD=true \
    --env K6_OTEL_GRPC_EXPORTER_ENDPOINT=localhost:4317 --env K6_OTEL_GRPC_EXPORTER_INSECURE=true --env K6_OTEL_METRIC_PREFIX=k6_ \
    --vus $vus --duration $duration \
    $testFile

# Run the result analyzer script
npx ts-node $resultAnalyzerFolder/src/main.ts $outputFile

echo "Result file can be found at '$outputFile'."
