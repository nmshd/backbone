#!/usr/bin/sh

/opt/mssql-tools/bin/sqlcmd -b -V16 -S localhost -U SA -P Passw0rd -Q "SELECT TABLE_SCHEMA + \".\" + TABLE_NAME as NAME FROM enmeshed.INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'" | tail --lines=+3 | head --lines=-2 > /tmp/tables.list
echo "" > /tmp/dump.sql
</tmp/tables.list xargs -I{} sh -c "printf \"\n{}\n\" >>/tmp/dump.sql && /opt/mssql-tools/bin/sqlcmd -b -V16 -P Passw0rd -S localhost -U SA -Q \"SET NOCOUNT ON; SELECT * FROM enmeshed.{}\" -s',' >> /tmp/dump.sql"