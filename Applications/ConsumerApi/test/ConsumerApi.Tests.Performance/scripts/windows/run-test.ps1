param (
    [string]$s
)

# Check if the scenario is provided; if not, prompt the user
if (-not $s) {
    $s = Read-Host "Enter the scenario name"
}

$k6Arguments = $args

# Generate a timestamp `t` in the format YYYYMMDD-HHmmSS
$t = Get-Date -Format "yyyyMMdd-HHmmss"

# Construct the file paths and commands
$testFile = ".\dist\$($s).test.js"
$outputFile = "k6-outputs\$($t)-$($s).csv"
$resultAnalyzerFolder = ".\tools\result-analyzer"

# Run the `npx webpack` command
npx webpack

Set-Location tools\result-analyzer

npm install

Set-Location ..\..

New-Item -Path "k6-outputs" -ItemType Directory -Force

# Check the exit code of the webpack command
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Webpack failed with exit code $LASTEXITCODE." -ForegroundColor Red
    exit $LASTEXITCODE
}

# Check if the test file exists
if (-not (Test-Path $testFile)) {
    Write-Host "Error: Test file '$testFile' does not exist." -ForegroundColor Red
    exit 1
}

try {
    # Run the `k6` command with additional arguments
    k6 run $testFile -v -o "csv=$outputFile" @k6Arguments
}
finally {
    Set-Location $resultAnalyzerFolder
    
    # Run the result analyzer script
    npx tsx \src\main.js $outputFile

    Set-Location ..\..
}

