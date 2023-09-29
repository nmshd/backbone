USE [enmeshed]
GO

/*++++++++++++++++++++++++++++++++++++++++++++++++++++ Create Schemas +++++++++++++++++++++++++++++++++++++++++++++++++++++*/
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

/*+++++++++++++++++++++++++++++++++++++++++++++++++ Set Schema Owners ++++++++++++++++++++++++++++++++++++++++++++++++++*/
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

/*+++++++++++++++++++++++++++++++++++++++++++++++++ Set Authorizations +++++++++++++++++++++++++++++++++++++++++++++++++*/
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
