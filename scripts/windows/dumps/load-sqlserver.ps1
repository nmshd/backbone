# Parameters with default values (can be overridden by arguments)
param (
    [string]$Hostname = "host.docker.internal",
    [string]$Username = "sa",
    [string]$Password = "Passw0rd"
    [string]$DbName = "enmeshed",
    [string]$DumpFile = "enmeshed.bacpac",
)

$ContainerName = "tmp-mssql-server"

docker run -d --rm --name $ContainerName -v $PSScriptRoot\dump-files:/dump ormico/sqlpackage sleep infinity

docker exec -it $ContainerName /opt/mssql-tools/bin/sqlcmd -S $Hostname -U $Username -P $Password -Q "DROP DATABASE IF EXISTS $DbName"
docker exec -ti $ContainerName sqlpackage /Action:Import /TargetServerName:$Hostname /TargetDatabaseName:$DbName /SourceFile:$ContainerDumpDir/$DumpFile /TargetUser:$Username /TargetPassword:$Password /TargetTrustServerCertificate:True

if ($LASTEXITCODE -eq 0) {
    Write-Host "Database import successful."
}

docker stop $ContainerName
