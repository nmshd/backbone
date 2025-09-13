# Parameters with default values (can be overridden by arguments)
param (
    [string]$Hostname = "host.docker.internal",
    [string]$Username = "SA",
    [string]$Password = "Passw0rd",
    [string]$DbName = "enmeshed"
)

$DumpFile = "enmeshed.bacpac"

docker run --rm -v "$PSScriptRoot\dump-files:/dump" ormico/sqlpackage sqlpackage /Action:Export /SourceServerName:$Hostname /SourceDatabaseName:$DbName /TargetFile:/dump/$DumpFile /SourceUser:$Username /SourcePassword:$Password /SourceTrustServerCertificate:True

if ($LASTEXITCODE -ne 0) {
    throw "Error: Database export to $DumpFile failed."
}

if (-not (Test-Path $PSScriptRoot\dump-files\$DumpFile)) {
    throw "Snapshot file not found in the 'snapshots' folder."
}

Write-Host "Database export to $DumpFile successful."
