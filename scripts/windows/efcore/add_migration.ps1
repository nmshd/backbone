Param(
    [parameter(Mandatory)][ValidateSet("AdminUi", "Challenges", "Devices", "Files", "Messages", "Quotas", "Relationships", "Synchronization", "Tokens")] $moduleName,
    [parameter(Mandatory)] $migrationName,
    [parameter(Mandatory)][ValidateSet("SqlServer", "Postgres", "")] $provider
)

$environment="dbmigrations-" + $provider.ToLower()
$repoRoot = git rev-parse --show-toplevel
$dbContextName = "${moduleName}DbContext"
$adminUiProject = "$repoRoot\AdminUi\src\AdminUi"
$consumerApiProject = "$repoRoot\ConsumerApi"

function AddMigration {    
    param (
        $provider
    )

    switch($moduleName){
        "AdminUi" {
            New-Item env:"${moduleName}__Infrastructure__SqlDatabase__Provider" -Value $provider -Force | Out-Null

            $migrationProject = "$repoRoot\AdminUi\src\AdminUi.Infrastructure.Database.$provider"
            $startupProject = $adminUiProject
        }
        Default {
            New-Item env:"Modules__${moduleName}__Infrastructure__SqlDatabase__Provider" -Value $provider -Force | Out-Null

            $migrationProject = "$repoRoot\Modules\$moduleName\src\$moduleName.Infrastructure.Database.$provider"
            $startupProject = $consumerApiProject
        }
    }

    $cmd = "dotnet ef migrations add --startup-project '$startupProject' --project '$migrationProject' --context $dbContextName --output-dir Migrations --verbose $migrationName -- --environment $environment"

    Write-Host "Executing '$cmd'..."
    Invoke-Expression $cmd
    CompileModels $provider
}

function CompileModels {
    param (
        $provider
    )

    switch($moduleName){
        "AdminUi" {
            $optimizationProject = $adminUiProject
            $outputDir = "$repoRoot\$moduleName\src\$moduleName.Infrastructure\CompiledModels\$provider"
            $namespace = "$moduleName.Infrastructure.CompiledModels.$provider"
        }
        Default {
            $optimizationProject = $consumerApiProject
            $outputDir = "$repoRoot\Modules\$moduleName\src\$moduleName.Infrastructure\CompiledModels\$provider"
            $namespace = "Backbone.Modules.$moduleName.Infrastructure.CompiledModels.$provider"
        }
    }

    $cmdOptimizeDbContext = "dotnet ef dbcontext optimize --project '$optimizationProject' --context $dbContextName --output-dir $outputDir --namespace $namespace"

    Write-Host "Compiling '$provider' models for '$moduleName'..."
    Invoke-Expression $cmdOptimizeDbContext
}

switch ($provider) {
    "SqlServer" { AddMigration $provider }
    "Postgres" { AddMigration $provider }
    "" { 
        AddMigration "SqlServer" 
        AddMigration "Postgres" 
    }
}