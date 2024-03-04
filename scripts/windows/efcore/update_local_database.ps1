Param(
    [parameter(Mandatory)][ValidateSet("AdminApi", "Challenges", "Devices", "Files", "Messages", "Quotas", "Relationships", "Synchronization", "Tokens")] $moduleName,
    [parameter(Mandatory)] $migrationName,
    [parameter(Mandatory)][ValidateSet("SqlServer", "Postgres", "")] $provider
)

$environment="dbmigrations-" + $provider.ToLower()
$repoRoot = git rev-parse --show-toplevel
$dbContextName = "${moduleName}DbContext"
$adminApiProject = "$repoRoot\AdminApi\src\AdminApi"
$consumerApiProject = "$repoRoot\ConsumerApi"

function UpdateLocalDatabase {    
    param (
        $provider
    )

    switch($moduleName){
        "AdminApi"{
            New-Item env:"${moduleName}__Infrastructure__SqlDatabase__Provider" -Value $provider -Force | Out-Null

            $migrationProject = "$repoRoot\AdminApi\src\AdminApi.Infrastructure.Database.$provider"
            $startupProject = $adminApiProject
        }
        Default {
            New-Item env:"Modules__${moduleName}__Infrastructure__SqlDatabase__Provider" -Value $provider -Force | Out-Null

            $migrationProject = "$repoRoot\Modules\$moduleName\src\$moduleName.Infrastructure.Database.$provider"
            $startupProject = $consumerApiProject
        }
    }

    $moduleNameLowercase = $moduleName.ToLower()
    switch ($provider) {
        "SqlServer" { $connectionString = """Server=localhost;Database=enmeshed;User Id=$moduleNameLowercase;Password=Passw0rd;TrustServerCertificate=True""" }
        "Postgres" { $connectionString = """Server=localhost;Database=enmeshed;User ID=$moduleNameLowercase;Password=Passw0rd""" }
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
