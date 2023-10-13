IF NOT EXISTS(SELECT *
FROM sys.server_principals
WHERE name = 'challenges')
BEGIN
	CREATE LOGIN challenges WITH PASSWORD = 'Passw0rd'
	PRINT 'Login "challenges" created' ;
END
IF NOT EXISTS(SELECT *
FROM sys.server_principals
WHERE name = 'devices')
BEGIN
	CREATE LOGIN devices WITH PASSWORD = 'Passw0rd'
	PRINT 'Login "devices" created' ;
END
IF NOT EXISTS(SELECT *
FROM sys.server_principals
WHERE name = 'files')
BEGIN
	CREATE LOGIN files WITH PASSWORD = 'Passw0rd'
	PRINT 'Login "files" created' ;
END
IF NOT EXISTS(SELECT *
FROM sys.server_principals
WHERE name = 'messages')
BEGIN
	CREATE LOGIN messages WITH PASSWORD = 'Passw0rd'
	PRINT 'Login "messages" created' ;
END
IF NOT EXISTS(SELECT *
FROM sys.server_principals
WHERE name = 'relationships')
BEGIN
	CREATE LOGIN relationships WITH PASSWORD = 'Passw0rd'
	PRINT 'Login "relationships" created' ;
END
IF NOT EXISTS(SELECT *
FROM sys.server_principals
WHERE name = 'synchronization')
BEGIN
	CREATE LOGIN synchronization WITH PASSWORD = 'Passw0rd'
	PRINT 'Login "synchronization" created' ;
END
IF NOT EXISTS(SELECT *
FROM sys.server_principals
WHERE name = 'tokens')
BEGIN
	CREATE LOGIN tokens WITH PASSWORD = 'Passw0rd'
	PRINT 'Login "tokens" created' ;
END
IF NOT EXISTS(SELECT *
FROM sys.server_principals
WHERE name = 'quotas')
BEGIN
	CREATE LOGIN quotas WITH PASSWORD = 'Passw0rd'
	PRINT 'Login "quotas" created' ;
END
IF NOT EXISTS(SELECT *
FROM sys.server_principals
WHERE name = 'adminUi')
BEGIN
	CREATE LOGIN adminUi WITH PASSWORD = 'Passw0rd'
	PRINT 'Login "adminUi" created' ;
END
GO
