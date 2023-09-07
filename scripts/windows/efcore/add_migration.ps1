Param(
    [parameter(Mandatory)][ValidateSet('Challenges', 'Devices', 'Files', "Messages", "Relationships", "Synchronization", "Tokens", "Quotas")] $moduleName,
    [parameter(Mandatory)] $migrationName,
    [parameter(Mandatory)][ValidateSet("SqlServer", "Postgres", "")] $provider
)

$environment="dbmigrations-" + $provider.ToLower()
$repoRoot = git rev-parse --show-toplevel
$dbContextName = "${moduleName}DbContext"
$startupProject = "$repoRoot\ConsumerApi"
$adminUiProject = "$repoRoot\AdminUi\src\AdminUi"

function CompileModelsAdminUi {
    $cmdAdminUiDbContext = "dotnet ef dbcontext optimize --project $adminUiProject --context AdminUiDbContext --output-dir $repoRoot\AdminUi\src\AdminUi.Infrastructure\CompiledModels --namespace AdminUi.Infrastructure.CompiledModels"
    $cmdDevicesDbContext = "dotnet ef dbcontext optimize --project $adminUiProject --context DevicesDbContext --output-dir $repoRoot\Modules\Devices\src\Devices.Infrastructure\CompiledModels --namespace Devices.Infrastructure.CompiledModels"
    Write-Host "Compiling models for '$adminUiProject'..."
    Invoke-Expression $cmdAdminUiDbContext
    Invoke-Expression $cmdDevicesDbContext
}

function AddMigration {    
    param (
        $provider
    )

    New-Item env:"Modules__${moduleName}__Infrastructure__SqlDatabase__Provider" -Value $provider -Force | Out-Null

    $migrationProject = "$repoRoot\Modules\$moduleName\src\$moduleName.Infrastructure.Database.$provider"

    $cmd = "dotnet ef migrations add --startup-project '$startupProject' --project '$migrationProject' --context $dbContextName --output-dir Migrations --verbose $migrationName -- --environment $environment"
    
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

CompileModelsAdminUi