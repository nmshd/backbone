docker cp dump_sqlserver.sh bkb-mssql_server:/tmp/dump_sqlserver.sh
docker exec -it bkb-mssql_server /tmp/dump_sqlserver.sh
docker cp bkb-mssql_server:/tmp/dump.sql ./dumps/enmeshed.csv