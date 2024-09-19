/* Declare Variables */
declare @NumberOfIdentities int;
declare @NumberOfConnectorIdentities int;
declare @NumberOfAppIdentities int;

declare @NumberOfDevices int;
declare @NumberOfAppDevices int;
declare @NumberOfConnectorDevices int;
declare @NumberOfAppIdentitiesWithMoreThanOneDevice int;
declare @NumberOfConnectorIdentitiesWithMoreThanOneDevice int;
declare @AvgNumberOfDevicesPerAppIdentity float;
declare @AvgNumberOfDevicesPerConnectorIdentity float;

declare @AvgNumberOfSentMessagesPerApp float;
declare @AvgNumberOfSentMessagesPerConnector float;
declare @AvgNumberOfRecievedMessagesPerApp float;
declare @AvgNumberOfRecievedMessagesPerConnector float;
declare @NumberOfMessagesWithMoreThanOneRecipient int;
declare @AvgNumberOfRecipientsPerMessage float;

declare @AvgNumberOfTokensPerApp float;
declare @AvgNumberOfTokensPerConnector float;

declare @AvgNumberOfRelationshipTemplatesPerApp float;
declare @AvgNumberOfRelationshipTemplatesPerConnector float;

declare @AvgNumberOfDatawalletModificationsPerApp float;
declare @AvgNumberOfDatawalletModificationsPerConnector float;

declare @AvgNumberOfFilesPerApp float;
declare @AvgNumberOfFilesPerConnector float;

declare @AvgSizeOfDatawalletModificationContent float;
declare @AvgSizeOfMessageContentPerApp float;
declare @AvgSizeOfMessageContentPerConnector float;
declare @AvgSizeOfTokenContentPerApp float;
declare @AvgSizeOfTokenContentPerConnector float;
declare @AvgSizeOfRelationshipTemplateContentPerApp float;
declare @AvgSizeOfRelationshipTemplateContentPerConnector float;

/* Identities */
select @NumberOfIdentities = count(*)
from Devices.Identities

select @NumberOfAppIdentities = count(*)
from Devices.Identities
where ClientId = 'bird-wallet'

set @NumberOfConnectorIdentities = @NumberOfIdentities - @NumberOfAppIdentities

/* Devices */
select @NumberOfDevices = count(*)
from Devices.Devices

select @NumberOfAppDevices = count(*)
from Devices.Devices d
where (select ClientId from Devices.Identities where Address = d.IdentityAddress) = 'bird-wallet'

set @NumberOfConnectorDevices = @NumberOfDevices - @NumberOfAppDevices

select @NumberOfAppIdentitiesWithMoreThanOneDevice = count(*)
from (
	select count(IdentityAddress) as cnt
	from Devices.Devices d
	where (select ClientId from Devices.Identities where Address = d.IdentityAddress) = 'bird-wallet'
	group by IdentityAddress
) d
where cnt > 1

select @NumberOfConnectorIdentitiesWithMoreThanOneDevice = count(*)
from (
	select count(IdentityAddress) as cnt
	from Devices.Devices d
	where (select ClientId from Devices.Identities where Address = d.IdentityAddress) <> 'bird-wallet'
	group by IdentityAddress
) d
where cnt > 1

select @AvgNumberOfDevicesPerAppIdentity = avg(cnt)
from (
	select convert(decimal, count(IdentityAddress)) as cnt
	from Devices.Devices d
	where (select ClientId from Devices.Identities where Address = d.IdentityAddress) = 'bird-wallet'
	group by IdentityAddress
) d

select @AvgNumberOfDevicesPerConnectorIdentity = avg(cnt)
from (
	select convert(decimal, count(IdentityAddress)) as cnt
	from Devices.Devices d
	where (select ClientId from Devices.Identities where Address = d.IdentityAddress) <> 'bird-wallet'
	group by IdentityAddress
) d

/* Messages */
select @AvgNumberOfSentMessagesPerApp = avg(msgCount)
from (
	select convert(decimal, count(CreatedBy)) as msgCount
	from Messages.Messages m
	where (select ClientId from Devices.Identities where Address = m.CreatedBy) = 'bird-wallet'
	group by CreatedBy
) msg

select @AvgNumberOfSentMessagesPerConnector = avg(msgCount)
from (
	select convert(decimal, count(CreatedBy)) as msgCount
	from Messages.Messages m
	where (select ClientId from Devices.Identities where Address = m.CreatedBy) <> 'bird-wallet'
	group by CreatedBy
) msg

select @AvgNumberOfRecievedMessagesPerApp = avg(msgId)
from (
	select convert(decimal, count(MessageId)) as msgId
	from Messages.RecipientInformation r
	where (select ClientId from Devices.Identities where Address = r.Address) = 'bird-wallet'
	group by Address
) msg

select @AvgNumberOfRecievedMessagesPerConnector = avg(msgId)
from (
	select convert(decimal, count(MessageId)) as msgId
	from Messages.RecipientInformation r
	where (select ClientId from Devices.Identities where Address = r.Address) <> 'bird-wallet'
	group by Address
) msg

select @NumberOfMessagesWithMoreThanOneRecipient = count(*)
from (
	select count(Address) as cnt
	from Messages.RecipientInformation
	group by MessageId
) msg
where cnt > 1

select @AvgNumberOfRecipientsPerMessage = avg(cnt)
from (
	select convert(decimal, count(Address)) as cnt
	from Messages.RecipientInformation
	group by MessageId
) msg

/* Tokens */
select @AvgNumberOfTokensPerApp = avg(tkCount)
from (
	select convert(decimal, count(*)) as tkCount
	from Tokens.Tokens t
	where (select ClientId from Devices.Identities where Address = t.CreatedBy) = 'bird-wallet'
	group by CreatedBy
) t

select @AvgNumberOfTokensPerConnector = avg(tkCount)
from (
	select convert(decimal, count(*)) as tkCount
	from Tokens.Tokens t
	where (select ClientId from Devices.Identities where Address = t.CreatedBy) <> 'bird-wallet'
	group by CreatedBy
) t

/* Relationship Templates */
select @AvgNumberOfRelationshipTemplatesPerApp = avg(rtCount)
from (
	select convert(decimal, count(*)) as rtCount
	from Relationships.RelationshipTemplates rt
	where (select ClientId from Devices.Identities where Address = rt.CreatedBy) = 'bird-wallet'
	group by CreatedBy
) t

select @AvgNumberOfRelationshipTemplatesPerConnector = avg(rtCount)
from (
	select convert(decimal, count(*)) as rtCount
	from Relationships.RelationshipTemplates rt
	where (select ClientId from Devices.Identities where Address = rt.CreatedBy) <> 'bird-wallet'
	group by CreatedBy
) t

/* Datawallet Modifications */
select @AvgNumberOfDatawalletModificationsPerApp = avg(rtCount)
from (
	select convert(decimal, count(*)) as rtCount
	from Synchronization.DatawalletModifications dm
	where (select ClientId from Devices.Identities where Address = dm.CreatedBy) = 'bird-wallet'
	group by CreatedBy
) t

select @AvgNumberOfDatawalletModificationsPerConnector = avg(rtCount)
from (
	select convert(decimal, count(*)) as rtCount
	from Synchronization.DatawalletModifications dm
	where (select ClientId from Devices.Identities where Address = dm.CreatedBy) <> 'bird-wallet'
	group by CreatedBy
) t

/* Files */
select @AvgNumberOfFilesPerApp = avg(cnt)
from (
	select convert(decimal, count(CreatedBy)) as "cnt"
	from Files.FileMetadata fm
	where (select ClientId from Devices.Identities where Address = fm.CreatedBy) = 'bird-wallet'
	group by CreatedBy
) m

select @AvgNumberOfFilesPerConnector = avg(cnt)
from (
	select convert(decimal, count(CreatedBy)) as "cnt"
	from Files.FileMetadata fm
	where (select ClientId from Devices.Identities where Address = fm.CreatedBy) <> 'bird-wallet'
	group by CreatedBy
) m

/* Sizes */
select @AvgSizeOfDatawalletModificationContent = avg(length)
from (
	select convert(decimal, datalength(EncryptedPayload)) as length
	from Synchronization.DatawalletModifications dm
	where (select ClientId from Devices.Identities where Address = dm.CreatedBy) = 'bird-wallet'
) m

select @AvgSizeOfMessageContentPerApp = avg(length)
from (
	select convert(decimal, datalength(Body)) as length
	from Messages.Messages m
	where (select ClientId from Devices.Identities where Address = m.CreatedBy) = 'bird-wallet'
) m

select @AvgSizeOfMessageContentPerConnector = avg(length)
from (
	select convert(decimal, datalength(Body)) as length
	from Messages.Messages m
	where (select ClientId from Devices.Identities where Address = m.CreatedBy) <> 'bird-wallet'
) m

select @AvgSizeOfTokenContentPerApp = avg(length)
from (
	select convert(decimal, datalength(Content)) as length
	from Tokens.Tokens t
	where (select ClientId from Devices.Identities where Address = t.CreatedBy) = 'bird-wallet'
) m

select @AvgSizeOfTokenContentPerConnector = avg(length)
from (
	select convert(decimal, datalength(Content)) as length
	from Tokens.Tokens t
	where (select ClientId from Devices.Identities where Address = t.CreatedBy) <> 'bird-wallet'
) m

select @AvgSizeOfRelationshipTemplateContentPerApp = avg(length)
from (
	select convert(decimal, datalength(Content)) as length
	from Relationships.RelationshipTemplates rt
	where (select ClientId from Devices.Identities where Address = rt.CreatedBy) = 'bird-wallet'
) m

select @AvgSizeOfRelationshipTemplateContentPerConnector = avg(length)
from (
	select convert(decimal, datalength(Content)) as length
	from Relationships.RelationshipTemplates rt
	where (select ClientId from Devices.Identities where Address = rt.CreatedBy) <> 'bird-wallet'
) m

/* Print values */
declare @result table(Category nvarchar(100), Value float);
insert into @result values
('Number of Identities', @NumberOfIdentities),
('Number of Connector Identities', @NumberOfConnectorIdentities),
('Number of App Identities', @NumberOfAppIdentities),

('Number of Devices', @NumberOfDevices),
('Number of App Devices', @NumberOfAppDevices),
('Number of Connector Devices', @NumberOfConnectorDevices),
('Number of App Identities with more than one Device', @NumberOfAppIdentitiesWithMoreThanOneDevice),
('Number of Connector Identities with more than one Device', @NumberOfConnectorIdentitiesWithMoreThanOneDevice),
(N'⌀ Number of Devices per App Identity', @AvgNumberOfDevicesPerAppIdentity),
(N'⌀ Number of Devices per Connector Identity', @AvgNumberOfDevicesPerConnectorIdentity),

(N'⌀ Number of Sent Messages per App Identity', @AvgNumberOfSentMessagesPerApp),
(N'⌀ Number of Sent Messages per Connector Identity', @AvgNumberOfSentMessagesPerConnector),
(N'⌀ Number of Recieved Messages per App Identity', @AvgNumberOfRecievedMessagesPerApp),
(N'⌀ Number of Recieved Messages per Connector Identity', @AvgNumberOfRecievedMessagesPerConnector),
('Number of Messages with more than 1 Recipient', @NumberOfMessagesWithMoreThanOneRecipient),
(N'⌀ Number of Recipients per Message', @AvgNumberOfRecipientsPerMessage),

(N'⌀ Number of Tokens per App Identity', @AvgNumberOfTokensPerApp),
(N'⌀ Number of Tokens per Connector Identity', @AvgNumberOfTokensPerConnector),

(N'⌀ Number of Relationship Templates per App Identity', @AvgNumberOfRelationshipTemplatesPerApp),
(N'⌀ Number of Relationship Templates per Connector Identity', @AvgNumberOfRelationshipTemplatesPerConnector),

(N'⌀ Number of Datawallet Modifications per App Identity', @AvgNumberOfDatawalletModificationsPerApp),
(N'⌀ Number of Datawallet Modifications per Connector Identity', @AvgNumberOfDatawalletModificationsPerConnector),

(N'⌀ Number of Files per App Identity', @AvgNumberOfFilesPerApp),
(N'⌀ Number of Files per Connector Identity', @AvgNumberOfFilesPerConnector),

(N'⌀ Size of Datawallet Modifications Content', @AvgSizeOfDatawalletModificationContent),
(N'⌀ Size of an App Message Content', @AvgSizeOfMessageContentPerApp),
(N'⌀ Size of an Connector Message Content', @AvgSizeOfMessageContentPerConnector),
(N'⌀ Size of an App Token Content', @AvgSizeOfTokenContentPerApp),
(N'⌀ Size of an Connector Token Content', @AvgSizeOfTokenContentPerConnector),
(N'⌀ Size of an App Relationship Template Content', @AvgSizeOfRelationshipTemplateContentPerApp),
(N'⌀ Size of an Connector Relationship Template Content', @AvgSizeOfRelationshipTemplateContentPerConnector)

select *
from @result
