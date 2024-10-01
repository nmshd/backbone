# PowerShell Script to Extract a Zip File from 'snapshots' Folder

param(
    [string]$SnapshotName,
    [string]$Hostname = "host.docker.internal",
    [string]$Username = "postgres",
    [string]$Password = "admin",
    [string]$DbName = "enmeshed"
)

Add-Type -AssemblyName System.IO.Compression.FileSystem

# Check if the snapshot is provided; if not, prompt the user
if (-not $SnapshotName) {
    $SnapshotName = Read-Host "Enter the name of the zip file (without extension). The file should be present in the 'snapshots' folder"
}

$repoRoot = git rev-parse --show-toplevel
$snapshotZipPath = "$repoRoot\Applications\ConsumerApi\test\ConsumerApi.Tests.Performance\snapshots\$SnapshotName.zip"

if (-not (Test-Path $snapshotZipPath)) {
    throw "Snapshot file '$SnapshotName' not found in the 'snapshots' folder."
}

# Extract the zip file
try {
    Write-Host "Extracting '$SnapshotName'..."
    [System.IO.Compression.ZipFile]::ExtractToDirectory($snapshotZipPath, "$repoRoot\scripts\windows\dumps\dump-files", 1)

}
catch {
    Write-Host "An error occurred during extraction: $_"
}

& "$repoRoot\scripts\windows\dumps\load-postgres.ps1" -Hostname $Hostname -Username $Username -Password $Password -DbName $DbName -Dumpfile enmeshed.pg
