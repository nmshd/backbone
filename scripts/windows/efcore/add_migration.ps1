Param(
    [parameter(Mandatory)][ValidateSet("AdminUi", "Challenges", "Devices", "Files", "Messages", "Quotas", "Relationships", "Synchronization", "Tokens")] $moduleName,
    [parameter(Mandatory)] $migrationName,
    [parameter(Mandatory)][ValidateSet("SqlServer", "Postgres", "")] $provider
)

$repoRoot = git rev-parse --show-toplevel
$dbContextName = "${moduleName}DbContext"
$adminUiProject = "$repoRoot\AdminUi\src\AdminUi"
$consumerApiProject = "$repoRoot\ConsumerApi"
$startupProject = If ($moduleName -eq "AdminUi") { $adminUiProject } Else { $consumerApiProject }

function AddMigration {    
    param (
        $provider
    )

    switch ($moduleName) {
        "AdminUi" {
            New-Item env:"Infrastructure__SqlDatabase__Provider" -Value $provider -Force | Out-Null

            $migrationProject = "$repoRoot\AdminUi\src\AdminUi.Infrastructure.Database.$provider"
        }
        Default {
            New-Item env:"Modules__${moduleName}__Infrastructure__SqlDatabase__Provider" -Value $provider -Force | Out-Null

            $migrationProject = "$repoRoot\Modules\$moduleName\src\$moduleName.Infrastructure.Database.$provider"
        }
    }

    $cmd = "dotnet ef migrations add --no-build --startup-project '$startupProject' --project '$migrationProject' --context $dbContextName --output-dir Migrations --verbose $migrationName"

    Write-Host "Executing '$cmd'..."
    Invoke-Expression $cmd
}

dotnet build /property:WarningLevel=0 $startupProject

switch ($provider) {
    "SqlServer" { AddMigration $provider }
    "Postgres" { AddMigration $provider }
    "" { 
        AddMigration "SqlServer" 
        AddMigration "Postgres" 
    }
}
