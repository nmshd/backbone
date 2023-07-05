Param(
    [parameter(Mandatory)][ValidateSet('Challenges', 'Devices', 'Files', "Messages", "Relationships", "Synchronization", "Tokens", "Quotas")] $moduleName,
    [parameter(Mandatory)][ValidateSet("SqlServer", "Postgres", "")] $provider
)

$environment="dbmigrations-" + $provider.ToLower()
$repoRoot = git rev-parse --show-toplevel
$startupProject = "$repoRoot\ConsumerApi"
$dbContextName = "${moduleName}DbContext"

function RemoveMigration {    
    param (
        $provider
    )

    New-Item env:"Modules__${moduleName}__Infrastructure__SqlDatabase__Provider" -Value $provider -Force | Out-Null

    $migrationProject = "$repoRoot\Modules\$moduleName\src\$moduleName.Infrastructure.Database.$provider"

    $cmd = "dotnet ef migrations remove --startup-project $startupProject --project $migrationProject --context $dbContextName --force -- --environment $environment"

    Write-Host "Executing '$cmd'..."
    Invoke-Expression $cmd
}

switch ($provider) {
    "SqlServer" { RemoveMigration $provider }
    "Postgres" { RemoveMigration $provider }
    "" { 
        RemoveMigration "SqlServer" 
        RemoveMigration "Postgres" 
    }
}