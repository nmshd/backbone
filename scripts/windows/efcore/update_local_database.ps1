Param(
    [parameter(Mandatory)][ValidateSet("AdminApi", "Announcements", "Challenges", "Devices", "Files", "Messages", "Quotas", "Relationships", "Synchronization", "Tokens")] $moduleName,
    [parameter(Mandatory)] $migrationName,
    [parameter(Mandatory)][ValidateSet("SqlServer", "Postgres", "")] $provider
)

$environment="dbmigrations-" + $provider.ToLower()
$repoRoot = git rev-parse --show-toplevel
$dbContextName = "${moduleName}DbContext"
$startupProject = "$repoRoot\Applications\DatabaseMigrator\src\DatabaseMigrator"

function UpdateLocalDatabase {    
    param (
        $provider
    )

    New-Item env:"Infrastructure__SqlDatabase__Provider" -Value $provider -Force | Out-Null

    switch($moduleName){
        "AdminApi"{
            $migrationProject = "$repoRoot\AdminApi\src\AdminApi.Infrastructure.Database.$provider"
        }
        Default {
            $migrationProject = "$repoRoot\Modules\$moduleName\src\$moduleName.Infrastructure.Database.$provider"
        }
    }

    $moduleNameLowercase = $moduleName.ToLower()
    switch ($provider) {
        "SqlServer" { $connectionString = """Server=localhost;Database=enmeshed;User Id=sa;Password=Passw0rd;TrustServerCertificate=True""" }
        "Postgres" { $connectionString = """Server=localhost;Database=enmeshed;User ID=postgres;Password=admin""" }
    }

    $cmd = "dotnet ef database update $migrationName --startup-project $startupProject --project $migrationProject --verbose --context $dbContextName --connection $connectionString -- --environment $environment"

    Write-Host "Executing '$cmd' ..."
    Invoke-Expression $cmd
}

switch ($provider) {
    "SqlServer" { UpdateLocalDatabase $provider }
    "Postgres" { UpdateLocalDatabase $provider }
    "" { 
        UpdateLocalDatabase "SqlServer" 
        UpdateLocalDatabase "Postgres" 
    }
}
