Param(
    [parameter(Mandatory)][ValidateSet("AdminApi", "Announcements", "Challenges", "Devices", "Files", "Messages", "Quotas", "Relationships", "Synchronization", "Tokens")] $moduleName,
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
$startupProject = "$repoRoot\Applications\DatabaseMigrator\src\DatabaseMigrator"

function AddMigration {    
    param (
        $provider
    )

    New-Item env:"Infrastructure__SqlDatabase__Provider" -Value $provider -Force | Out-Null

    switch ($moduleName) {
        "AdminApi" {
            $migrationProject = "$repoRoot\Applications\AdminApi\src\AdminApi.Infrastructure.Database.$provider"
        }
        Default {
            $migrationProject = "$repoRoot\Modules\$moduleName\src\$moduleName.Infrastructure.Database.$provider"
        }
    }

    $cmd = "dotnet ef migrations add --no-build --startup-project '$startupProject' --project '$migrationProject' --context $dbContextName --output-dir Migrations --verbose $migrationName"

    Write-Host "Executing '$cmd'..."
    Invoke-Expression $cmd
}

dotnet build $startupProject

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
