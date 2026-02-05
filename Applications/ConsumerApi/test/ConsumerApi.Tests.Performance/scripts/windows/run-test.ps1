param (
    [Parameter(Mandatory=$true)][string]$scenario,
    [string]$baseUrl = $env:NMSHD_TEST_BASEURL ? $env:NMSHD_TEST_BASEURL : "http://localhost:8081/",
    [string]$clientId = $env:NMSHD_TEST_CLIENTID ? $env:NMSHD_TEST_CLIENTID : "test",
    [string]$clientSecret = $env:NMSHD_TEST_CLIENTSECRET ? $env:NMSHD_TEST_CLIENTSECRET : "test",
    [string]$apiVersion = "v2",
    [string]$vus = "1",
    [string]$duration = "10s"
)

echo "Scenario: $scenario"
echo "Base URL: $baseUrl"
echo "Client ID: $clientId"
echo "Client Secret: $clientSecret"
echo "API Version: $apiVersion"
echo "VUs: $vus"
echo "Duration: $duration"

echo "-------------------------------"

exit

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
    --env baseUrl=$baseUrl --env clientId=$clientId --env clientSecret=$clientSecret --env apiVersion=$apiVersion `
    --vus $vus --duration $duration `
    $testFile

# Run the result analyzer script
npx ts-node $resultAnalyzerFolder\src\main.ts $outputFile

Write-Host "Result file can be found at '$outputFile'."
