Param(
    [parameter(Mandatory)][ValidateSet("AdminUi", "Challenges", "Devices", "Files", "Messages", "Quotas", "Relationships", "Synchronization", "Tokens")] $moduleName,
    [parameter(Mandatory)][ValidateSet("SqlServer", "Postgres", "")] $provider
)

$repoRoot = git rev-parse --show-toplevel
$dbContextName = "${moduleName}DbContext"
$adminUiProject = "$repoRoot\AdminUi\src\AdminUi"
$consumerApiProject = "$repoRoot\ConsumerApi"

function CompileModels {
    switch($provider) {
        "Postgres" {
            Write-Host "Skipping Postgres compiled models..."
            return;
        }
    }

    switch($moduleName){
        "AdminUi" {
            $startupProject = $adminUiProject
            $outputDir = "$repoRoot\$moduleName\src\$moduleName.Infrastructure\CompiledModels\$provider"
            $namespace = "$moduleName.Infrastructure.CompiledModels.$provider"
        }
        Default {
            $startupProject = $consumerApiProject
            $outputDir = "$repoRoot\Modules\$moduleName\src\$moduleName.Infrastructure\CompiledModels\$provider"
            $namespace = "Backbone.Modules.$moduleName.Infrastructure.CompiledModels.$provider"
        }
    }

    $cmdOptimizeDbContext = "dotnet ef dbcontext optimize --project '$startupProject' --context $dbContextName --output-dir $outputDir --namespace $namespace"

    Write-Host "Compiling '$provider' models for '$moduleName'..."
    Invoke-Expression $cmdOptimizeDbContext
}

CompileModels