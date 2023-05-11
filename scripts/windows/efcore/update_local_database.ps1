Param(
    [parameter(Mandatory)][ValidateSet('Challenges', 'Devices', 'Files', "Messages", "Relationships", "Synchronization", "Tokens")] $moduleName,
    [parameter(Mandatory)] $migrationName,
    [parameter(Mandatory)][ValidateSet("SqlServer", "Postgres", "")] $provider
)

$environment="dbmigrations-" + $provider.ToLower()
$repoRoot = git rev-parse --show-toplevel
$startupProject = "$repoRoot\ConsumerApi"
$dbContextName = "${moduleName}DbContext"

function UpdateLocalDatabase {    
    param (
        $provider
    )

    $migrationProject = "$repoRoot\Modules\$moduleName\src\$moduleName.Infrastructure.Database.$provider"

    New-Item env:"Modules__${moduleName}__Infrastructure__SqlDatabase__Provider" -Value $provider -Force | Out-Null

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
