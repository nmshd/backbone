/* Server Configuration */
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

IF NOT (EXISTS (SELECT name
FROM master.dbo.sysdatabases
WHERE (name = '[enmeshed]' OR name = 'enmeshed')))
BEGIN
	CREATE DATABASE [enmeshed]
	PRINT 'Database "enmeshed" created' ;
END
GO

USE [enmeshed]
GO

/********************************************** Database Configuration **********************************************/
/*++++++++++++++++++++++++++++++++++++++++++++++++++++ Schemas +++++++++++++++++++++++++++++++++++++++++++++++++++++*/
IF NOT EXISTS ( SELECT *
FROM sys.schemas
WHERE name = N'Challenges' )
BEGIN
	EXEC('CREATE SCHEMA [Challenges]')
	PRINT 'Schema "Challenges" created' ;
END

IF NOT EXISTS ( SELECT *
FROM sys.schemas
WHERE name = N'Devices' )
BEGIN
	EXEC('CREATE SCHEMA [Devices]')
	PRINT 'Schema "Devices" created' ;
END

IF NOT EXISTS ( SELECT *
FROM sys.schemas
WHERE name = N'Messages' )
BEGIN
	EXEC('CREATE SCHEMA [Messages]')
	PRINT 'Schema "Messages" created' ;
END

IF NOT EXISTS ( SELECT *
FROM sys.schemas
WHERE name = N'Synchronization' )
BEGIN
	EXEC('CREATE SCHEMA [Synchronization]')
	PRINT 'Schema "Synchronization" created' ;
END

IF NOT EXISTS ( SELECT *
FROM sys.schemas
WHERE name = N'Tokens' )
BEGIN
	EXEC('CREATE SCHEMA [Tokens]')
	PRINT 'Schema "Tokens" created' ;
END

IF NOT EXISTS ( SELECT *
FROM sys.schemas
WHERE name = N'Relationships' )
BEGIN
	EXEC('CREATE SCHEMA [Relationships]')
	PRINT 'Schema "Relationships" created' ;
END

IF NOT EXISTS ( SELECT *
FROM sys.schemas
WHERE name = N'Files' )
BEGIN
	EXEC('CREATE SCHEMA [Files]')
	PRINT 'Schema "Files" created' ;
END

IF NOT EXISTS ( SELECT *
FROM sys.schemas
WHERE name = N'Quotas' )
BEGIN
	EXEC('CREATE SCHEMA [Quotas]')
	PRINT 'Schema "Quotas" created' ;
END

IF NOT EXISTS ( SELECT *
FROM sys.schemas
WHERE name = N'AdminUi' )
BEGIN
	EXEC('CREATE SCHEMA [AdminUi]')
	PRINT 'Schema "AdminUi" created' ;
END

/*+++++++++++++++++++++++++++++++++++++++++++++++++++++ Users ++++++++++++++++++++++++++++++++++++++++++++++++++++++*/
IF NOT EXISTS (SELECT *
FROM sys.database_principals
WHERE name = 'challenges')
BEGIN
	CREATE USER challenges FOR LOGIN challenges
	PRINT 'User "challenges" created' ;
END

IF NOT EXISTS (SELECT *
FROM sys.database_principals
WHERE name = 'devices')
BEGIN
	CREATE USER devices FOR LOGIN devices
	PRINT 'User "devices" created' ;
END

IF NOT EXISTS (SELECT *
FROM sys.database_principals
WHERE name = 'messages')
BEGIN
	CREATE USER messages FOR LOGIN messages
	PRINT 'User "messages" created' ;
END

IF NOT EXISTS (SELECT *
FROM sys.database_principals
WHERE name = 'synchronization')
BEGIN
	CREATE USER synchronization FOR LOGIN synchronization
	PRINT 'User "synchronization" created' ;
END

IF NOT EXISTS (SELECT *
FROM sys.database_principals
WHERE name = 'tokens')
BEGIN
	CREATE USER tokens FOR LOGIN tokens
	PRINT 'User "tokens" created' ;
END

IF NOT EXISTS (SELECT *
FROM sys.database_principals
WHERE name = 'files')
BEGIN
	CREATE USER files FOR LOGIN files
	PRINT 'User "files" created' ;
END

IF NOT EXISTS (SELECT *
FROM sys.database_principals
WHERE name = 'relationships')
BEGIN
	CREATE USER relationships FOR LOGIN relationships
	PRINT 'User "relationships" created' ;
END


IF NOT EXISTS (SELECT *
FROM sys.database_principals
WHERE name = 'quotas')
BEGIN
	CREATE USER quotas FOR LOGIN quotas
	PRINT 'User "Quotas" created' ;
END

IF NOT EXISTS (SELECT *
FROM sys.database_principals
WHERE name = 'adminUi')
BEGIN
	CREATE USER adminUi FOR LOGIN adminUi
	PRINT 'User "adminUi" created' ;
END

GO

/*+++++++++++++++++++++++++++++++++++++++++++++++++ Authorizations +++++++++++++++++++++++++++++++++++++++++++++++++*/
PRINT 'Start changing authorizations' ;

GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE, REFERENCES, VIEW DEFINITION ON SCHEMA::Challenges TO challenges;
GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE, REFERENCES, VIEW DEFINITION ON SCHEMA::Devices TO devices;
GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE, REFERENCES, VIEW DEFINITION ON SCHEMA::Files TO files;
GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE, REFERENCES, VIEW DEFINITION ON SCHEMA::Messages TO messages;
GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE, REFERENCES, VIEW DEFINITION ON SCHEMA::Quotas TO quotas;
GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE, REFERENCES, VIEW DEFINITION ON SCHEMA::Relationships TO relationships;
GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE, REFERENCES, VIEW DEFINITION ON SCHEMA::Synchronization TO synchronization;
GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE, REFERENCES, VIEW DEFINITION ON SCHEMA::Tokens TO tokens;
GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE, REFERENCES, VIEW DEFINITION ON SCHEMA::AdminUi TO adminUi;

GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::AdminUi TO relationships /* we temporarily grant this permission because it is required by relationships in order to delete the AdminUi.RelationshipOverviews view*/
GRANT SELECT, REFERENCES ON SCHEMA::Relationships TO messages
GRANT SELECT, REFERENCES ON SCHEMA::Challenges TO devices
GRANT SELECT ON SCHEMA::Challenges TO quotas
GRANT SELECT ON SCHEMA::Devices TO quotas
GRANT SELECT ON SCHEMA::Files TO quotas
GRANT SELECT ON SCHEMA::Messages TO quotas
GRANT SELECT ON SCHEMA::Relationships TO quotas
GRANT SELECT ON SCHEMA::Synchronization TO quotas
GRANT SELECT ON SCHEMA::Tokens TO quotas
GRANT SELECT ON SCHEMA::Relationships TO synchronization
GRANT SELECT ON SCHEMA::Relationships TO adminUi
GRANT SELECT ON SCHEMA::Files TO adminUi
GRANT SELECT ON SCHEMA::Messages TO adminUi
GRANT SELECT ON SCHEMA::Challenges TO adminUi
GRANT SELECT ON SCHEMA::Synchronization TO adminUi
GRANT SELECT ON SCHEMA::Devices TO adminUi
GRANT SELECT ON SCHEMA::Tokens TO adminUi
GRANT SELECT ON SCHEMA::Quotas TO adminUi
GRANT CREATE VIEW TO adminUi
GRANT SELECT ON SCHEMA::Tokens TO quotas
PRINT 'Finished changing authorizations' ;
GO
