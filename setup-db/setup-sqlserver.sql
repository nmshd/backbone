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
	CREATE USER challenges FOR LOGIN challenges	WITH DEFAULT_SCHEMA = Challenges
	PRINT 'User "challenges" created' ;
END

IF NOT EXISTS (SELECT *
FROM sys.database_principals
WHERE name = 'devices')
BEGIN
	CREATE USER devices FOR LOGIN devices WITH DEFAULT_SCHEMA = Devices
	PRINT 'User "devices" created' ;
END

IF NOT EXISTS (SELECT *
FROM sys.database_principals
WHERE name = 'messages')
BEGIN
	CREATE USER messages FOR LOGIN messages	WITH DEFAULT_SCHEMA = Messages
	PRINT 'User "messages" created' ;
END

IF NOT EXISTS (SELECT *
FROM sys.database_principals
WHERE name = 'synchronization')
BEGIN
	CREATE USER synchronization FOR LOGIN synchronization	WITH DEFAULT_SCHEMA = Synchronization
	PRINT 'User "synchronization" created' ;
END

IF NOT EXISTS (SELECT *
FROM sys.database_principals
WHERE name = 'tokens')
BEGIN
	CREATE USER tokens FOR LOGIN tokens	WITH DEFAULT_SCHEMA = Tokens
	PRINT 'User "tokens" created' ;
END

IF NOT EXISTS (SELECT *
FROM sys.database_principals
WHERE name = 'files')
BEGIN
	CREATE USER files FOR LOGIN files	WITH DEFAULT_SCHEMA = Files
	PRINT 'User "files" created' ;
END

IF NOT EXISTS (SELECT *
FROM sys.database_principals
WHERE name = 'relationships')
BEGIN
	CREATE USER relationships FOR LOGIN relationships	WITH DEFAULT_SCHEMA = Relationships
	PRINT 'User "relationships" created' ;
END


IF NOT EXISTS (SELECT *
FROM sys.database_principals
WHERE name = 'quotas')
BEGIN
	CREATE USER quotas FOR LOGIN quotas	WITH DEFAULT_SCHEMA = Quotas
	PRINT 'User "Quotas" created' ;
END

IF NOT EXISTS (SELECT *
FROM sys.database_principals
WHERE name = 'adminUi')
BEGIN
	CREATE USER adminUi FOR LOGIN adminUi WITH DEFAULT_SCHEMA = AdminUi
	PRINT 'User "adminUi" created' ;
END

GO

/*+++++++++++++++++++++++++++++++++++++++++++++++++ Schema Owners ++++++++++++++++++++++++++++++++++++++++++++++++++*/
PRINT 'Start changing schema owners' ;
ALTER AUTHORIZATION ON SCHEMA::Challenges TO challenges
ALTER AUTHORIZATION ON SCHEMA::Devices TO devices
ALTER AUTHORIZATION ON SCHEMA::Messages TO messages
ALTER AUTHORIZATION ON SCHEMA::Synchronization TO synchronization
ALTER AUTHORIZATION ON SCHEMA::Tokens TO tokens
ALTER AUTHORIZATION ON SCHEMA::Relationships TO relationships
ALTER AUTHORIZATION ON SCHEMA::Files TO files
ALTER AUTHORIZATION ON SCHEMA::Quotas TO quotas
ALTER AUTHORIZATION ON SCHEMA::AdminUi TO adminUi
PRINT 'Finished changing schema owners' ;
GO

/*+++++++++++++++++++++++++++++++++++++++++++++++++ Authorizations +++++++++++++++++++++++++++++++++++++++++++++++++*/
PRINT 'Start changing authorizations' ;

GRANT CREATE TABLE TO challenges, devices, messages, synchronization, tokens, relationships, files, quotas, adminUi
GRANT CREATE FUNCTION TO relationships
GO

DENY SELECT, INSERT, UPDATE, DELETE ON SCHEMA::Challenges TO synchronization, devices, messages, tokens, relationships, files, quotas
DENY SELECT, INSERT, UPDATE, DELETE ON SCHEMA::Synchronization TO challenges, devices, messages, tokens, relationships, files, quotas
DENY SELECT, INSERT, UPDATE, DELETE ON SCHEMA::Messages TO challenges, synchronization, devices, tokens, relationships, files, quotas
DENY SELECT, INSERT, UPDATE, DELETE ON SCHEMA::Devices TO challenges, synchronization, messages, tokens, relationships, files, quotas
DENY SELECT, INSERT, UPDATE, DELETE ON SCHEMA::Tokens TO challenges, synchronization, devices, messages, relationships, files, quotas
DENY SELECT, INSERT, UPDATE, DELETE ON SCHEMA::Relationships TO challenges, synchronization, devices, messages, tokens, files, quotas
DENY SELECT, INSERT, UPDATE, DELETE ON SCHEMA::Files TO challenges, synchronization, devices, messages, tokens, relationships, quotas
DENY SELECT, INSERT, UPDATE, DELETE ON SCHEMA::Quotas TO challenges, synchronization, devices, messages, tokens, relationships, files

GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::AdminUi TO relationships /* we temporarily grant this permission because it is required by relationships in order to delete the AdminUi.RelationshipOverviews view*/
GRANT SELECT, REFERENCES ON SCHEMA::Relationships TO messages
GRANT SELECT, REFERENCES ON SCHEMA::Challenges TO devices
GRANT SELECT ON SCHEMA::Messages TO quotas
GRANT SELECT ON SCHEMA::Files TO quotas
GRANT SELECT ON SCHEMA::Relationships TO quotas
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
