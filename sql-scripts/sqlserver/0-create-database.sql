IF NOT (EXISTS (SELECT name
FROM master.dbo.sysdatabases
WHERE (name = '[enmeshed]' OR name = 'enmeshed')))
BEGIN
	CREATE DATABASE [enmeshed]
	PRINT 'Database "enmeshed" created' ;
END
GO
