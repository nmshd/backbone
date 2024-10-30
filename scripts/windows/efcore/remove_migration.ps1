Param(
    [parameter(Mandatory)][ValidateSet("AdminApi", "Announcements", "Challenges", "Devices", "Files", "Messages", "Quotas", "Relationships", "Synchronization", "Tokens")] $moduleName,
    [parameter(Mandatory)][ValidateSet("SqlServer", "Postgres", "")] $provider
)

$repoRoot = git rev-parse --show-toplevel
$dbContextName = "${moduleName}DbContext"
$startupProject = "$repoRoot\Applications\DatabaseMigrator\src\DatabaseMigrator"

function RemoveMigration {    
    param (
        $provider
    )

    New-Item env:"Infrastructure__SqlDatabase__Provider" -Value $provider -Force | Out-Null

    switch($moduleName){
        "AdminApi" {
            $migrationProject = "$repoRoot\AdminApi\src\AdminApi.Infrastructure.Database.$provider"
        }
        Default {
            $migrationProject = "$repoRoot\Modules\$moduleName\src\$moduleName.Infrastructure.Database.$provider"
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
