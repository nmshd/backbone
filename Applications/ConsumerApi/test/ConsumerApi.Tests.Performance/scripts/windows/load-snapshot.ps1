# PowerShell Script to Extract a Zip File from 'snapshots' Folder

param(
    [string]$SnapshotName,
    [string]$Hostname = "host.docker.internal",
    [string]$Username = "postgres",
    [string]$Password = "admin",
    [string]$DbName = "enmeshed"
)

# Check if the snapshot is provided; if not, prompt the user
if (-not $SnapshotName) {
    $SnapshotName = Read-Host "Enter the snapshot name"
}

# Get the current script location
$scriptPath = $PSScriptRoot
$rootPath = Resolve-Path "$scriptPath\..\.."
$snapshotsFolder = Join-Path $rootPath "snapshots"
$snapshotFilePath = Join-Path $snapshotsFolder ($SnapshotName + ".zip")
$repoRoot = Resolve-Path $scriptPath\..\..\..\..\..\..

# Check if the file exists
Write-Host $snapshotFilePath
if (-not (Test-Path $snapshotFilePath)) {
    Write-Host "Snapshot file '$SnapshotName' not found in the 'snapshots' folder."
    exit 1
}


# Extract the zip file
try {
    Write-Host "Extracting '$SnapshotName'..."
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::ExtractToDirectory($snapshotFilePath, $snapshotsFolder, 1)
    Write-Host "Extraction complete. Files are in '$snapshotsFolder'."
    Move-Item -Path (Join-Path $snapshotsFolder $SnapshotName "enmeshed.pg") -Destination (Join-Path $repoRoot "scripts\dumps\dump-files") -Force

}
catch {
    Write-Host "An error occurred during extraction: $_"
}

$loadPostgres = Join-Path $repoRoot "scripts\dumps\load-postgres.ps1"
& $loadPostgres -Hostname $Hostname -Username $Username -Password $Password -DbName $DbName -Dumpfile enmeshed.pg