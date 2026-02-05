param (
    [Parameter(Mandatory=$true)][string]$scenario,
    [string]$vus = "1",
    [string]$duration = "10s"
)

# Stop the script when a cmdlet or a native command fails
$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true

$k6Arguments = $args
$t = Get-Date -Format "yyyyMMdd-HHmmss"
$testFile = ".\dist\$($scenario).test.js"
$outputFile = "k6-outputs\$($t)-$($scenario).csv"
$resultAnalyzerFolder = ".\tools\result-analyzer"

npx webpack

Set-Location tools\result-analyzer
npm install
Set-Location ..\..

New-Item -Path "k6-outputs" -ItemType Directory -Force

k6 run `
    --tag testid=$t `
    --out "csv=$outputFile" --out opentelemetry `
    --env K6_WEB_DASHBOARD_EXPORT=html-report.html --env K6_WEB_DASHBOARD=true `
    --env K6_OTEL_GRPC_EXPORTER_ENDPOINT=localhost:4317 --env K6_OTEL_GRPC_EXPORTER_INSECURE=true --env K6_OTEL_METRIC_PREFIX=k6_ `
    --vus $vus --duration $duration `
    $testFile

# Run the result analyzer script
npx ts-node $resultAnalyzerFolder\src\main.ts $outputFile

Write-Host "Result file can be found at '$outputFile'."
