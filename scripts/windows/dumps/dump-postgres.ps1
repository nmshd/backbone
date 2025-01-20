# Parameters with default values (can be overridden by arguments)
param (
    [string]$Hostname = "host.docker.internal",
    [string]$Username = "postgres",
    [string]$Password = "admin",
    [string]$DbName = "enmeshed",
    [string]$DumpFile = "enmeshed.pg"
)

docker run --rm -v "$PSScriptRoot\dump-files:/dump" --env PGPASSWORD="admin" postgres pg_dump -h $Hostname -U $Username $DbName -f /dump/$DumpFile

if ($LASTEXITCODE -ne 0) {
    throw "Error: Database export to $DumpFile failed."
}

if (-not (Test-Path $PSScriptRoot\dump-files\$DumpFile)) {
    throw "Snapshot file not found in the 'snapshots' folder."
}

Write-Host "Database export to $DumpFile successful."
