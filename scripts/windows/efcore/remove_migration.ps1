Param(
    [parameter(Mandatory)][ValidateSet("AdminApi", "Challenges", "Devices", "Files", "Messages", "Quotas", "Relationships", "Synchronization", "Tokens")] $moduleName,
    [parameter(Mandatory)][ValidateSet("SqlServer", "Postgres", "")] $provider
)

$repoRoot = git rev-parse --show-toplevel
$dbContextName = "${moduleName}DbContext"
$adminApiProject = "$repoRoot\AdminApi\src\AdminApi"
$consumerApiProject = "$repoRoot\ConsumerApi"

function RemoveMigration {    
    param (
        $provider
    )

    switch($moduleName){
        "AdminApi" {
            New-Item env:"Infrastructure__SqlDatabase__Provider" -Value $provider -Force | Out-Null

            $migrationProject = "$repoRoot\AdminApi\src\AdminApi.Infrastructure.Database.$provider"
            $startupProject = $adminApiProject
        }
        Default {
            New-Item env:"Modules__${moduleName}__Infrastructure__SqlDatabase__Provider" -Value $provider -Force | Out-Null

            $migrationProject = "$repoRoot\Modules\$moduleName\src\$moduleName.Infrastructure.Database.$provider"
            $startupProject = $consumerApiProject
        }
    }

    $cmd = "dotnet ef migrations remove --startup-project $startupProject --project $migrationProject --context $dbContextName --force"

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
