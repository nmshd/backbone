Param(
    [parameter(Mandatory)][ValidateSet('Challenges', 'Devices', 'Files', "Messages", "Relationships", "Synchronization", "Tokens")] $moduleName,
    [parameter(Mandatory)] $migrationName,
    [parameter(Mandatory)][ValidateSet("SqlServer", "Postgres", "")] $provider
)

$env:ASPNETCORE_ENVIRONMENT = 'Local'

$repoRoot = git rev-parse --show-toplevel
$dbContextName = "${moduleName}DbContext"
$startupProject = "$repoRoot\Backbone.API"

function AddMigration {    
    param (
        $provider
    )

    New-Item env:"Modules__${moduleName}__Infrastructure__SqlDatabase__Provider" -Value $provider -Force | Out-Null

    $migrationProject = "$repoRoot\Modules\$moduleName\src\$moduleName.Infrastructure.Database.$provider"

    $cmd = "dotnet ef migrations add --startup-project $startupProject --project $migrationProject --context $dbContextName --output-dir Migrations --verbose $migrationName"

    Write-Host "Executing '$cmd'..."
    Invoke-Expression $cmd
}

switch ($provider) {
    "SqlServer" { AddMigration $provider }
    "Postgres" { AddMigration $provider }
    "" { 
        AddMigration "SqlServer" 
        AddMigration "Postgres" 
    }
}
