Param(
    [parameter(Mandatory)][ValidateSet("AdminUi", "Challenges", "Devices", "Files", "Messages", "Quotas", "Relationships", "Synchronization", "Tokens")] $moduleName,
    [parameter(Mandatory)][ValidateSet("SqlServer", "Postgres", "")] $provider
)

$environment="dbmigrations-" + $provider.ToLower()
$repoRoot = git rev-parse --show-toplevel
$dbContextName = "${moduleName}DbContext"
$adminUiProject = "$repoRoot\AdminUi\src\AdminUi"
$consumerApiProject = "$repoRoot\ConsumerApi"

function RemoveMigration {    
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
