docker exec -it bkb-mssql_server /opt/mssql-tools/bin/sqlcmd -b -V16 -P Passw0rd -S localhost -U SA  -Q "BACKUP DATABASE enmeshed TO DISK = N'/tmp/enmeshed.bak' with NOREWIND"
docker cp bkb-mssql_server:/tmp/enmeshed.bak ./dump-files/enmeshed.bak
