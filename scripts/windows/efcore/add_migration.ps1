Param(
    [parameter(Mandatory)][ValidateSet("AdminApi", "Challenges", "Devices", "Files", "Messages", "Quotas", "Relationships", "Synchronization", "Tokens")] $moduleName,
    [parameter(Mandatory)] $migrationName,
    [parameter(Mandatory)][ValidateSet("s", "p", "SqlServer", "Postgres", "")] $provider
)

$provider = switch ($provider) {
    "s" { "SqlServer" }
    "p" { "Postgres" }
    Default { $provider }
}
$repoRoot = git rev-parse --show-toplevel
$dbContextName = "${moduleName}DbContext"
$adminApiProject = "$repoRoot\AdminApi\src\AdminApi"
$consumerApiProject = "$repoRoot\ConsumerApi"
$startupProject = If ($moduleName -eq "AdminApi") { $adminApiProject } Else { $consumerApiProject }

function AddMigration {    
    param (
        $provider
    )

    switch ($moduleName) {
        "AdminApi" {
            New-Item env:"Infrastructure__SqlDatabase__Provider" -Value $provider -Force | Out-Null

            $migrationProject = "$repoRoot\AdminApi\src\AdminApi.Infrastructure.Database.$provider"
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
    "SqlServer" { 
        AddMigration "SqlServer" 
    }
    "Postgres" { 
        AddMigration "Postgres" 
    }
    "" { 
        AddMigration "SqlServer" 
        AddMigration "Postgres" 
    }
}
