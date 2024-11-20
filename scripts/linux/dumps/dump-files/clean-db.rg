--
-- PostgreSQL database dump
--

-- Dumped from database version 16.3 (Debian 16.3-1.pgdg120+1)
-- Dumped by pg_dump version 16.3 (Debian 16.3-1.pgdg120+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: AdminUi; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA "AdminUi";


ALTER SCHEMA "AdminUi" OWNER TO postgres;

--
-- Name: Challenges; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA "Challenges";


ALTER SCHEMA "Challenges" OWNER TO postgres;

--
-- Name: Devices; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA "Devices";


ALTER SCHEMA "Devices" OWNER TO postgres;

--
-- Name: Files; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA "Files";


ALTER SCHEMA "Files" OWNER TO postgres;

--
-- Name: Messages; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA "Messages";


ALTER SCHEMA "Messages" OWNER TO postgres;

--
-- Name: Quotas; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA "Quotas";


ALTER SCHEMA "Quotas" OWNER TO postgres;

--
-- Name: Relationships; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA "Relationships";


ALTER SCHEMA "Relationships" OWNER TO postgres;

--
-- Name: Synchronization; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA "Synchronization";


ALTER SCHEMA "Synchronization" OWNER TO postgres;

--
-- Name: Tokens; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA "Tokens";


ALTER SCHEMA "Tokens" OWNER TO postgres;

--
-- Name: getnumberofactiverelationshipsbetween(character varying, character varying); Type: FUNCTION; Schema: Relationships; Owner: postgres
--

CREATE FUNCTION "Relationships".getnumberofactiverelationshipsbetween(identitya character varying, identityb character varying) RETURNS integer
    LANGUAGE plpgsql
    AS $$
BEGIN
return (SELECT COUNT(r."Id") FROM "Relationships"."Relationships" r WHERE ((r."From" = identityA AND r."To" = identityB) OR (r."From" = identityB AND r."To" = identityA)) AND r."Status" IN (10, 20, 50));
END
$$;


ALTER FUNCTION "Relationships".getnumberofactiverelationshipsbetween(identitya character varying, identityb character varying) OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: Identities; Type: TABLE; Schema: Devices; Owner: postgres
--

CREATE TABLE "Devices"."Identities" (
    "Address" character varying(80) NOT NULL,
    "ClientId" character varying(200),
    "PublicKey" bytea NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "IdentityVersion" smallint NOT NULL,
    "TierIdBeforeDeletion" character(20),
    "TierId" character(20) NOT NULL,
    "DeletionGracePeriodEndsAt" timestamp with time zone,
    "Status" integer NOT NULL
);


ALTER TABLE "Devices"."Identities" OWNER TO postgres;

--
-- Name: OpenIddictApplications; Type: TABLE; Schema: Devices; Owner: postgres
--

CREATE TABLE "Devices"."OpenIddictApplications" (
    "Id" text NOT NULL,
    "DefaultTier" character(20) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "MaxIdentities" integer,
    "ApplicationType" character varying(50),
    "ClientId" character varying(100),
    "ClientSecret" text,
    "ClientType" character varying(50),
    "ConcurrencyToken" character varying(50),
    "ConsentType" character varying(50),
    "DisplayName" text,
    "DisplayNames" text,
    "JsonWebKeySet" text,
    "Permissions" text,
    "PostLogoutRedirectUris" text,
    "Properties" text,
    "RedirectUris" text,
    "Requirements" text,
    "Settings" text
);


ALTER TABLE "Devices"."OpenIddictApplications" OWNER TO postgres;

--
-- Name: Tiers; Type: TABLE; Schema: Devices; Owner: postgres
--

CREATE TABLE "Devices"."Tiers" (
    "Id" character(20) NOT NULL,
    "Name" character varying(30) NOT NULL,
    "CanBeUsedAsDefaultForClient" boolean DEFAULT true NOT NULL,
    "CanBeManuallyAssigned" boolean DEFAULT true NOT NULL
);


ALTER TABLE "Devices"."Tiers" OWNER TO postgres;

--
-- Name: ClientOverviews; Type: VIEW; Schema: AdminUi; Owner: postgres
--

CREATE VIEW "AdminUi"."ClientOverviews" AS
 SELECT clients."ClientId",
    clients."DisplayName",
    clients."DefaultTier" AS "DefaultTierId",
    tiers."Name" AS "DefaultTierName",
    clients."CreatedAt",
    ( SELECT count("Identities"."ClientId") AS count
           FROM "Devices"."Identities"
          WHERE (("Identities"."ClientId")::text = (clients."ClientId")::text)) AS "NumberOfIdentities",
    clients."MaxIdentities"
   FROM ("Devices"."OpenIddictApplications" clients
     LEFT JOIN "Devices"."Tiers" tiers ON ((tiers."Id" = clients."DefaultTier")));


ALTER VIEW "AdminUi"."ClientOverviews" OWNER TO postgres;

--
-- Name: AspNetUsers; Type: TABLE; Schema: Devices; Owner: postgres
--

CREATE TABLE "Devices"."AspNetUsers" (
    "Id" text NOT NULL,
    "DeviceId" character(20) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastLoginAt" timestamp with time zone,
    "UserName" character(20),
    "NormalizedUserName" character varying(256),
    "PasswordHash" text,
    "SecurityStamp" text,
    "ConcurrencyStamp" text,
    "LockoutEnd" timestamp with time zone,
    "LockoutEnabled" boolean NOT NULL,
    "AccessFailedCount" integer NOT NULL
);


ALTER TABLE "Devices"."AspNetUsers" OWNER TO postgres;

--
-- Name: Devices; Type: TABLE; Schema: Devices; Owner: postgres
--

CREATE TABLE "Devices"."Devices" (
    "Id" character(20) NOT NULL,
    "IdentityAddress" character varying(80) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CommunicationLanguage" character(2) DEFAULT 'en'::bpchar NOT NULL,
    "CreatedByDevice" character(20) NOT NULL,
    "DeletedAt" timestamp with time zone,
    "DeletedByDevice" character(20)
);


ALTER TABLE "Devices"."Devices" OWNER TO postgres;

--
-- Name: Datawallets; Type: TABLE; Schema: Synchronization; Owner: postgres
--

CREATE TABLE "Synchronization"."Datawallets" (
    "Id" character(20) NOT NULL,
    "Owner" character varying(80) NOT NULL,
    "Version" integer NOT NULL
);


ALTER TABLE "Synchronization"."Datawallets" OWNER TO postgres;

--
-- Name: IdentityOverviews; Type: VIEW; Schema: AdminUi; Owner: postgres
--

CREATE VIEW "AdminUi"."IdentityOverviews" AS
 SELECT identities."Address",
    identities."CreatedAt",
    users."LastLoginAt",
    identities."ClientId" AS "CreatedWithClient",
    datawallets."Version" AS "DatawalletVersion",
    identities."IdentityVersion",
    tiers."Id" AS "TierId",
    tiers."Name" AS "TierName",
    devices."NumberOfDevices"
   FROM (((("Devices"."Identities" identities
     LEFT JOIN ( SELECT "Devices"."IdentityAddress",
            count(*) AS "NumberOfDevices"
           FROM "Devices"."Devices"
          GROUP BY "Devices"."IdentityAddress") devices ON (((devices."IdentityAddress")::text = (identities."Address")::text)))
     LEFT JOIN ( SELECT devices_1."IdentityAddress",
            max(users_1."LastLoginAt") AS "LastLoginAt"
           FROM ("Devices"."AspNetUsers" users_1
             JOIN "Devices"."Devices" devices_1 ON ((devices_1."Id" = users_1."DeviceId")))
          GROUP BY devices_1."IdentityAddress") users ON (((users."IdentityAddress")::text = (identities."Address")::text)))
     LEFT JOIN "Synchronization"."Datawallets" datawallets ON (((datawallets."Owner")::text = (identities."Address")::text)))
     LEFT JOIN "Devices"."Tiers" tiers ON ((tiers."Id" = identities."TierId")));


ALTER VIEW "AdminUi"."IdentityOverviews" OWNER TO postgres;

--
-- Name: Attachments; Type: TABLE; Schema: Messages; Owner: postgres
--

CREATE TABLE "Messages"."Attachments" (
    "Id" character(20) NOT NULL,
    "MessageId" character(20) NOT NULL
);


ALTER TABLE "Messages"."Attachments" OWNER TO postgres;

--
-- Name: Messages; Type: TABLE; Schema: Messages; Owner: postgres
--

CREATE TABLE "Messages"."Messages" (
    "Id" character(20) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" character varying(80) NOT NULL,
    "CreatedByDevice" character(20) NOT NULL,
    "Body" bytea
);


ALTER TABLE "Messages"."Messages" OWNER TO postgres;

--
-- Name: MessageOverviews; Type: VIEW; Schema: AdminUi; Owner: postgres
--

CREATE VIEW "AdminUi"."MessageOverviews" AS
 SELECT "Messages"."Id" AS "MessageId",
    "Messages"."CreatedBy" AS "SenderAddress",
    "Messages"."CreatedByDevice" AS "SenderDevice",
    "Messages"."CreatedAt" AS "SendDate",
    count("Attachments"."Id") AS "NumberOfAttachments"
   FROM ("Messages"."Messages" "Messages"
     LEFT JOIN "Messages"."Attachments" "Attachments" ON (("Messages"."Id" = "Attachments"."MessageId")))
  GROUP BY "Messages"."Id", "Messages"."CreatedBy", "Messages"."CreatedByDevice", "Messages"."CreatedAt";


ALTER VIEW "AdminUi"."MessageOverviews" OWNER TO postgres;

--
-- Name: RelationshipAuditLog; Type: TABLE; Schema: Relationships; Owner: postgres
--

CREATE TABLE "Relationships"."RelationshipAuditLog" (
    "Id" character(20) NOT NULL,
    "Reason" integer NOT NULL,
    "OldStatus" integer,
    "NewStatus" integer NOT NULL,
    "CreatedBy" character varying(80) NOT NULL,
    "CreatedByDevice" character(20) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "RelationshipId" character(20)
);


ALTER TABLE "Relationships"."RelationshipAuditLog" OWNER TO postgres;

--
-- Name: Relationships; Type: TABLE; Schema: Relationships; Owner: postgres
--

CREATE TABLE "Relationships"."Relationships" (
    "Id" character(20) NOT NULL,
    "RelationshipTemplateId" character(20),
    "From" character varying(80) NOT NULL,
    "To" character varying(80) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "Status" integer NOT NULL,
    "CreationContent" bytea,
    "CreationResponseContent" bytea,
    "FromHasDecomposed" boolean NOT NULL,
    "ToHasDecomposed" boolean NOT NULL,
    CONSTRAINT ck_only_one_active_relationship_between_two_identities CHECK (("Relationships".getnumberofactiverelationshipsbetween("From", "To") <= 1))
);


ALTER TABLE "Relationships"."Relationships" OWNER TO postgres;

--
-- Name: RelationshipOverviews; Type: VIEW; Schema: AdminUi; Owner: postgres
--

CREATE VIEW "AdminUi"."RelationshipOverviews" AS
 SELECT "Relationships"."From",
    "Relationships"."To",
    "Relationships"."RelationshipTemplateId",
    "Relationships"."Status",
    "AuditLog1"."CreatedAt",
    "AuditLog1"."CreatedByDevice",
    "AuditLog2"."CreatedAt" AS "AnsweredAt",
    "AuditLog2"."CreatedByDevice" AS "AnsweredByDevice"
   FROM (("Relationships"."Relationships" "Relationships"
     LEFT JOIN "Relationships"."RelationshipAuditLog" "AuditLog1" ON ((("Relationships"."Id" = "AuditLog1"."RelationshipId") AND ("AuditLog1"."Reason" = 0))))
     LEFT JOIN "Relationships"."RelationshipAuditLog" "AuditLog2" ON ((("Relationships"."Id" = "AuditLog2"."RelationshipId") AND ("AuditLog2"."Reason" = 1))));


ALTER VIEW "AdminUi"."RelationshipOverviews" OWNER TO postgres;

--
-- Name: TierOverviews; Type: VIEW; Schema: AdminUi; Owner: postgres
--

CREATE VIEW "AdminUi"."TierOverviews" AS
 SELECT tiers."Id",
    tiers."Name",
    count(identities."TierId") AS "NumberOfIdentities",
    tiers."CanBeUsedAsDefaultForClient",
    tiers."CanBeManuallyAssigned"
   FROM ("Devices"."Tiers" tiers
     LEFT JOIN "Devices"."Identities" identities ON ((identities."TierId" = tiers."Id")))
  GROUP BY tiers."Id", tiers."Name", tiers."CanBeUsedAsDefaultForClient", tiers."CanBeManuallyAssigned";


ALTER VIEW "AdminUi"."TierOverviews" OWNER TO postgres;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: AdminUi; Owner: postgres
--

CREATE TABLE "AdminUi"."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE "AdminUi"."__EFMigrationsHistory" OWNER TO postgres;

--
-- Name: Challenges; Type: TABLE; Schema: Challenges; Owner: postgres
--

CREATE TABLE "Challenges"."Challenges" (
    "Id" character(20) NOT NULL,
    "ExpiresAt" timestamp with time zone NOT NULL,
    "CreatedBy" character varying(80),
    "CreatedByDevice" character(20)
);


ALTER TABLE "Challenges"."Challenges" OWNER TO postgres;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: Challenges; Owner: postgres
--

CREATE TABLE "Challenges"."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE "Challenges"."__EFMigrationsHistory" OWNER TO postgres;

--
-- Name: AspNetRoleClaims; Type: TABLE; Schema: Devices; Owner: postgres
--

CREATE TABLE "Devices"."AspNetRoleClaims" (
    "Id" integer NOT NULL,
    "RoleId" text NOT NULL,
    "ClaimType" text,
    "ClaimValue" text
);


ALTER TABLE "Devices"."AspNetRoleClaims" OWNER TO postgres;

--
-- Name: AspNetRoleClaims_Id_seq; Type: SEQUENCE; Schema: Devices; Owner: postgres
--

ALTER TABLE "Devices"."AspNetRoleClaims" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME "Devices"."AspNetRoleClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: AspNetRoles; Type: TABLE; Schema: Devices; Owner: postgres
--

CREATE TABLE "Devices"."AspNetRoles" (
    "Id" text NOT NULL,
    "Name" character varying(256),
    "NormalizedName" character varying(256),
    "ConcurrencyStamp" text
);


ALTER TABLE "Devices"."AspNetRoles" OWNER TO postgres;

--
-- Name: AspNetUserClaims; Type: TABLE; Schema: Devices; Owner: postgres
--

CREATE TABLE "Devices"."AspNetUserClaims" (
    "Id" integer NOT NULL,
    "UserId" text NOT NULL,
    "ClaimType" text,
    "ClaimValue" text
);


ALTER TABLE "Devices"."AspNetUserClaims" OWNER TO postgres;

--
-- Name: AspNetUserClaims_Id_seq; Type: SEQUENCE; Schema: Devices; Owner: postgres
--

ALTER TABLE "Devices"."AspNetUserClaims" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME "Devices"."AspNetUserClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: AspNetUserLogins; Type: TABLE; Schema: Devices; Owner: postgres
--

CREATE TABLE "Devices"."AspNetUserLogins" (
    "LoginProvider" text NOT NULL,
    "ProviderKey" text NOT NULL,
    "ProviderDisplayName" text,
    "UserId" text NOT NULL
);


ALTER TABLE "Devices"."AspNetUserLogins" OWNER TO postgres;

--
-- Name: AspNetUserRoles; Type: TABLE; Schema: Devices; Owner: postgres
--

CREATE TABLE "Devices"."AspNetUserRoles" (
    "UserId" text NOT NULL,
    "RoleId" text NOT NULL
);


ALTER TABLE "Devices"."AspNetUserRoles" OWNER TO postgres;

--
-- Name: AspNetUserTokens; Type: TABLE; Schema: Devices; Owner: postgres
--

CREATE TABLE "Devices"."AspNetUserTokens" (
    "UserId" text NOT NULL,
    "LoginProvider" text NOT NULL,
    "Name" text NOT NULL,
    "Value" text
);


ALTER TABLE "Devices"."AspNetUserTokens" OWNER TO postgres;

--
-- Name: IdentityDeletionProcessAuditLog; Type: TABLE; Schema: Devices; Owner: postgres
--

CREATE TABLE "Devices"."IdentityDeletionProcessAuditLog" (
    "Id" character(20) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "MessageKey" text NOT NULL,
    "IdentityAddressHash" bytea NOT NULL,
    "DeviceIdHash" bytea,
    "OldStatus" integer,
    "NewStatus" integer NOT NULL,
    "IdentityDeletionProcessId" character(20),
    "AdditionalData" text
);


ALTER TABLE "Devices"."IdentityDeletionProcessAuditLog" OWNER TO postgres;

--
-- Name: IdentityDeletionProcesses; Type: TABLE; Schema: Devices; Owner: postgres
--

CREATE TABLE "Devices"."IdentityDeletionProcesses" (
    "Id" character(20) NOT NULL,
    "Status" integer NOT NULL,
    "DeletionStartedAt" timestamp with time zone,
    "CreatedAt" timestamp with time zone NOT NULL,
    "ApprovalReminder1SentAt" timestamp with time zone,
    "ApprovalReminder2SentAt" timestamp with time zone,
    "ApprovalReminder3SentAt" timestamp with time zone,
    "ApprovedAt" timestamp with time zone,
    "ApprovedByDevice" character(20),
    "RejectedAt" timestamp with time zone,
    "RejectedByDevice" character(20),
    "CancelledAt" timestamp with time zone,
    "CancelledByDevice" character(20),
    "GracePeriodEndsAt" timestamp with time zone,
    "GracePeriodReminder1SentAt" timestamp with time zone,
    "GracePeriodReminder2SentAt" timestamp with time zone,
    "GracePeriodReminder3SentAt" timestamp with time zone,
    "IdentityAddress" character varying(80) DEFAULT ''::character varying NOT NULL
);


ALTER TABLE "Devices"."IdentityDeletionProcesses" OWNER TO postgres;

--
-- Name: OpenIddictAuthorizations; Type: TABLE; Schema: Devices; Owner: postgres
--

CREATE TABLE "Devices"."OpenIddictAuthorizations" (
    "Id" text NOT NULL,
    "ApplicationId" text,
    "ConcurrencyToken" character varying(50),
    "CreationDate" timestamp with time zone,
    "Properties" text,
    "Scopes" text,
    "Status" character varying(50),
    "Subject" character varying(400),
    "Type" character varying(50)
);


ALTER TABLE "Devices"."OpenIddictAuthorizations" OWNER TO postgres;

--
-- Name: OpenIddictScopes; Type: TABLE; Schema: Devices; Owner: postgres
--

CREATE TABLE "Devices"."OpenIddictScopes" (
    "Id" text NOT NULL,
    "ConcurrencyToken" character varying(50),
    "Description" text,
    "Descriptions" text,
    "DisplayName" text,
    "DisplayNames" text,
    "Name" character varying(200),
    "Properties" text,
    "Resources" text
);


ALTER TABLE "Devices"."OpenIddictScopes" OWNER TO postgres;

--
-- Name: OpenIddictTokens; Type: TABLE; Schema: Devices; Owner: postgres
--

CREATE TABLE "Devices"."OpenIddictTokens" (
    "Id" text NOT NULL,
    "ApplicationId" text,
    "AuthorizationId" text,
    "ConcurrencyToken" character varying(50),
    "CreationDate" timestamp with time zone,
    "ExpirationDate" timestamp with time zone,
    "Payload" text,
    "Properties" text,
    "RedemptionDate" timestamp with time zone,
    "ReferenceId" character varying(100),
    "Status" character varying(50),
    "Subject" character varying(400),
    "Type" character varying(50)
);


ALTER TABLE "Devices"."OpenIddictTokens" OWNER TO postgres;

--
-- Name: PnsRegistrations; Type: TABLE; Schema: Devices; Owner: postgres
--

CREATE TABLE "Devices"."PnsRegistrations" (
    "DeviceId" character(20) NOT NULL,
    "IdentityAddress" character varying(80) NOT NULL,
    "DevicePushIdentifier" character(20) NOT NULL,
    "Handle" character varying(200) NOT NULL,
    "AppId" text NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "Environment" integer NOT NULL
);


ALTER TABLE "Devices"."PnsRegistrations" OWNER TO postgres;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: Devices; Owner: postgres
--

CREATE TABLE "Devices"."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE "Devices"."__EFMigrationsHistory" OWNER TO postgres;

--
-- Name: FileMetadata; Type: TABLE; Schema: Files; Owner: postgres
--

CREATE TABLE "Files"."FileMetadata" (
    "Id" character(20) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" character varying(80) NOT NULL,
    "CreatedByDevice" character(20) NOT NULL,
    "ModifiedAt" timestamp with time zone NOT NULL,
    "ModifiedBy" character varying(80) NOT NULL,
    "ModifiedByDevice" character(20) NOT NULL,
    "DeletedAt" timestamp with time zone,
    "DeletedBy" character varying(80),
    "DeletedByDevice" character(20),
    "Owner" character varying(80) NOT NULL,
    "OwnerSignature" bytea NOT NULL,
    "CipherSize" bigint NOT NULL,
    "CipherHash" bytea NOT NULL,
    "ExpiresAt" timestamp with time zone NOT NULL,
    "EncryptedProperties" bytea NOT NULL
);


ALTER TABLE "Files"."FileMetadata" OWNER TO postgres;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: Files; Owner: postgres
--

CREATE TABLE "Files"."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE "Files"."__EFMigrationsHistory" OWNER TO postgres;

--
-- Name: RecipientInformation; Type: TABLE; Schema: Messages; Owner: postgres
--

CREATE TABLE "Messages"."RecipientInformation" (
    "Id" integer NOT NULL,
    "Address" character varying(80) NOT NULL,
    "EncryptedKey" bytea NOT NULL,
    "ReceivedAt" timestamp with time zone,
    "ReceivedByDevice" character(20),
    "MessageId" character(20) NOT NULL,
    "IsRelationshipDecomposedByRecipient" boolean DEFAULT false NOT NULL,
    "IsRelationshipDecomposedBySender" boolean DEFAULT false NOT NULL,
    "RelationshipId" character(20) DEFAULT ''::bpchar NOT NULL
);


ALTER TABLE "Messages"."RecipientInformation" OWNER TO postgres;

--
-- Name: RecipientInformation_Id_seq; Type: SEQUENCE; Schema: Messages; Owner: postgres
--

ALTER TABLE "Messages"."RecipientInformation" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME "Messages"."RecipientInformation_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: Messages; Owner: postgres
--

CREATE TABLE "Messages"."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE "Messages"."__EFMigrationsHistory" OWNER TO postgres;

--
-- Name: Identities; Type: TABLE; Schema: Quotas; Owner: postgres
--

CREATE TABLE "Quotas"."Identities" (
    "Address" character varying(80) NOT NULL,
    "TierId" character(20) NOT NULL
);


ALTER TABLE "Quotas"."Identities" OWNER TO postgres;

--
-- Name: IndividualQuotas; Type: TABLE; Schema: Quotas; Owner: postgres
--

CREATE TABLE "Quotas"."IndividualQuotas" (
    "Id" character(20) NOT NULL,
    "ApplyTo" character varying(80) NOT NULL,
    "MetricKey" character varying(50) NOT NULL,
    "Max" integer NOT NULL,
    "Period" integer NOT NULL
);


ALTER TABLE "Quotas"."IndividualQuotas" OWNER TO postgres;

--
-- Name: MetricStatuses; Type: TABLE; Schema: Quotas; Owner: postgres
--

CREATE TABLE "Quotas"."MetricStatuses" (
    "MetricKey" character varying(50) NOT NULL,
    "Owner" character varying(80) NOT NULL,
    "IsExhaustedUntil" timestamp with time zone NOT NULL
);


ALTER TABLE "Quotas"."MetricStatuses" OWNER TO postgres;

--
-- Name: TierQuotaDefinitions; Type: TABLE; Schema: Quotas; Owner: postgres
--

CREATE TABLE "Quotas"."TierQuotaDefinitions" (
    "Id" character(20) NOT NULL,
    "MetricKey" character varying(50) NOT NULL,
    "Max" integer NOT NULL,
    "Period" integer NOT NULL,
    "TierId" character(20)
);


ALTER TABLE "Quotas"."TierQuotaDefinitions" OWNER TO postgres;

--
-- Name: TierQuotas; Type: TABLE; Schema: Quotas; Owner: postgres
--

CREATE TABLE "Quotas"."TierQuotas" (
    "Id" character(20) NOT NULL,
    "DefinitionId" character(20),
    "ApplyTo" character varying(80) NOT NULL
);


ALTER TABLE "Quotas"."TierQuotas" OWNER TO postgres;

--
-- Name: Tiers; Type: TABLE; Schema: Quotas; Owner: postgres
--

CREATE TABLE "Quotas"."Tiers" (
    "Id" character(20) NOT NULL,
    "Name" character varying(30) NOT NULL
);


ALTER TABLE "Quotas"."Tiers" OWNER TO postgres;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: Quotas; Owner: postgres
--

CREATE TABLE "Quotas"."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE "Quotas"."__EFMigrationsHistory" OWNER TO postgres;

--
-- Name: RelationshipTemplateAllocations; Type: TABLE; Schema: Relationships; Owner: postgres
--

CREATE TABLE "Relationships"."RelationshipTemplateAllocations" (
    "Id" integer NOT NULL,
    "RelationshipTemplateId" character(20) NOT NULL,
    "AllocatedBy" character varying(80) NOT NULL,
    "AllocatedAt" timestamp with time zone NOT NULL,
    "AllocatedByDevice" character(20) NOT NULL
);


ALTER TABLE "Relationships"."RelationshipTemplateAllocations" OWNER TO postgres;

--
-- Name: RelationshipTemplateAllocations_Id_seq; Type: SEQUENCE; Schema: Relationships; Owner: postgres
--

ALTER TABLE "Relationships"."RelationshipTemplateAllocations" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME "Relationships"."RelationshipTemplateAllocations_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: RelationshipTemplates; Type: TABLE; Schema: Relationships; Owner: postgres
--

CREATE TABLE "Relationships"."RelationshipTemplates" (
    "Id" character(20) NOT NULL,
    "CreatedBy" character varying(80) NOT NULL,
    "CreatedByDevice" character(20) NOT NULL,
    "MaxNumberOfAllocations" integer,
    "ExpiresAt" timestamp with time zone,
    "Content" bytea,
    "CreatedAt" timestamp with time zone NOT NULL,
    "ForIdentity" character varying(80),
    "Password" bytea
);


ALTER TABLE "Relationships"."RelationshipTemplates" OWNER TO postgres;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: Relationships; Owner: postgres
--

CREATE TABLE "Relationships"."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE "Relationships"."__EFMigrationsHistory" OWNER TO postgres;

--
-- Name: DatawalletModifications; Type: TABLE; Schema: Synchronization; Owner: postgres
--

CREATE TABLE "Synchronization"."DatawalletModifications" (
    "Id" character(20) NOT NULL,
    "DatawalletId" character(20),
    "DatawalletVersion" integer NOT NULL,
    "Index" bigint NOT NULL,
    "ObjectIdentifier" character varying(100) NOT NULL,
    "PayloadCategory" character varying(50),
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" character varying(80) NOT NULL,
    "CreatedByDevice" character(20) NOT NULL,
    "Collection" character varying(50) NOT NULL,
    "Type" integer NOT NULL,
    "EncryptedPayload" bytea
);


ALTER TABLE "Synchronization"."DatawalletModifications" OWNER TO postgres;

--
-- Name: ExternalEvents; Type: TABLE; Schema: Synchronization; Owner: postgres
--

CREATE TABLE "Synchronization"."ExternalEvents" (
    "Id" character(20) NOT NULL,
    "Type" integer NOT NULL,
    "Index" bigint NOT NULL,
    "Owner" character varying(80) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "Payload" character varying(200) NOT NULL,
    "SyncErrorCount" smallint NOT NULL,
    "SyncRunId" character(20),
    "Context" character varying(20),
    "IsDeliveryBlocked" boolean DEFAULT false NOT NULL
);


ALTER TABLE "Synchronization"."ExternalEvents" OWNER TO postgres;

--
-- Name: SyncErrors; Type: TABLE; Schema: Synchronization; Owner: postgres
--

CREATE TABLE "Synchronization"."SyncErrors" (
    "Id" character(20) NOT NULL,
    "SyncRunId" character(20) NOT NULL,
    "ExternalEventId" character(20) NOT NULL,
    "ErrorCode" character varying(100) NOT NULL
);


ALTER TABLE "Synchronization"."SyncErrors" OWNER TO postgres;

--
-- Name: SyncRuns; Type: TABLE; Schema: Synchronization; Owner: postgres
--

CREATE TABLE "Synchronization"."SyncRuns" (
    "Id" character(20) NOT NULL,
    "Type" integer NOT NULL,
    "ExpiresAt" timestamp with time zone NOT NULL,
    "Index" bigint NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" character varying(80) NOT NULL,
    "CreatedByDevice" character(20) NOT NULL,
    "FinalizedAt" timestamp with time zone,
    "EventCount" integer NOT NULL
);


ALTER TABLE "Synchronization"."SyncRuns" OWNER TO postgres;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: Synchronization; Owner: postgres
--

CREATE TABLE "Synchronization"."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE "Synchronization"."__EFMigrationsHistory" OWNER TO postgres;

--
-- Name: Tokens; Type: TABLE; Schema: Tokens; Owner: postgres
--

CREATE TABLE "Tokens"."Tokens" (
    "Id" character(20) NOT NULL,
    "CreatedBy" character varying(80) NOT NULL,
    "CreatedByDevice" character(20) NOT NULL,
    "Content" bytea,
    "CreatedAt" timestamp with time zone NOT NULL,
    "ExpiresAt" timestamp with time zone NOT NULL,
    "ForIdentity" character varying(80),
    "Password" bytea
);


ALTER TABLE "Tokens"."Tokens" OWNER TO postgres;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: Tokens; Owner: postgres
--

CREATE TABLE "Tokens"."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE "Tokens"."__EFMigrationsHistory" OWNER TO postgres;

--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: AdminUi; Owner: postgres
--

COPY "AdminUi"."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20240701060320_Init	8.0.10
\.


--
-- Data for Name: Challenges; Type: TABLE DATA; Schema: Challenges; Owner: postgres
--

COPY "Challenges"."Challenges" ("Id", "ExpiresAt", "CreatedBy", "CreatedByDevice") FROM stdin;
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: Challenges; Owner: postgres
--

COPY "Challenges"."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20240701073944_Init	8.0.10
\.


--
-- Data for Name: AspNetRoleClaims; Type: TABLE DATA; Schema: Devices; Owner: postgres
--

COPY "Devices"."AspNetRoleClaims" ("Id", "RoleId", "ClaimType", "ClaimValue") FROM stdin;
\.


--
-- Data for Name: AspNetRoles; Type: TABLE DATA; Schema: Devices; Owner: postgres
--

COPY "Devices"."AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp") FROM stdin;
\.


--
-- Data for Name: AspNetUserClaims; Type: TABLE DATA; Schema: Devices; Owner: postgres
--

COPY "Devices"."AspNetUserClaims" ("Id", "UserId", "ClaimType", "ClaimValue") FROM stdin;
\.


--
-- Data for Name: AspNetUserLogins; Type: TABLE DATA; Schema: Devices; Owner: postgres
--

COPY "Devices"."AspNetUserLogins" ("LoginProvider", "ProviderKey", "ProviderDisplayName", "UserId") FROM stdin;
\.


--
-- Data for Name: AspNetUserRoles; Type: TABLE DATA; Schema: Devices; Owner: postgres
--

COPY "Devices"."AspNetUserRoles" ("UserId", "RoleId") FROM stdin;
\.


--
-- Data for Name: AspNetUserTokens; Type: TABLE DATA; Schema: Devices; Owner: postgres
--

COPY "Devices"."AspNetUserTokens" ("UserId", "LoginProvider", "Name", "Value") FROM stdin;
\.


--
-- Data for Name: AspNetUsers; Type: TABLE DATA; Schema: Devices; Owner: postgres
--

COPY "Devices"."AspNetUsers" ("Id", "DeviceId", "CreatedAt", "LastLoginAt", "UserName", "NormalizedUserName", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "LockoutEnd", "LockoutEnabled", "AccessFailedCount") FROM stdin;
106284e3-0e60-4d7c-881a-de57f36fd76f	DVClsoJUqY0d5nUV6Vo9	2024-11-15 09:25:40.03169+00	\N	USRb                	USRB	AQAAAAIAAYagAAAAECj6PGeaxg7roD/S5rpYqr3t1X6XUyyGtZjWpBmUHYL+WXxrlKbcYVwvRe8c3Hrhzw==	L5MQSGW3SRJTZKQWTGJXFHZJUGPGXZ4Y	0c223ddf-8f61-4e1f-9155-6e1685ce3129	\N	t	0
50525eb1-aa8c-4b56-b0d8-08c7230ab960	DVCSw43pOhKSxgg4fAHU	2024-11-15 09:25:40.029848+00	2024-11-15 09:36:53.279066+00	USRa                	USRA	AQAAAAIAAYagAAAAEOwmE+6HhhVbawD2g7HuOE2rk6JXVNdrEN/Qaa9xbJ6p82Dt6ATi6DK7SU2bwWO0Yw==	MY2SEPHZS4HSGSQ66RNKKFPGPQICTLKR	44270903-c94a-4550-bc31-137a989a0dba	\N	t	0
\.


--
-- Data for Name: Devices; Type: TABLE DATA; Schema: Devices; Owner: postgres
--

COPY "Devices"."Devices" ("Id", "IdentityAddress", "CreatedAt", "CommunicationLanguage", "CreatedByDevice", "DeletedAt", "DeletedByDevice") FROM stdin;
DVCSw43pOhKSxgg4fAHU	did:e:localhost:dids:0f3e40164b6c495c28674f	2024-11-15 09:25:40.02841+00	en	DVCSw43pOhKSxgg4fAHU	\N	\N
DVClsoJUqY0d5nUV6Vo9	did:e:localhost:dids:8234cca0160ff05c785636	2024-11-15 09:25:40.031677+00	en	DVClsoJUqY0d5nUV6Vo9	\N	\N
\.


--
-- Data for Name: Identities; Type: TABLE DATA; Schema: Devices; Owner: postgres
--

COPY "Devices"."Identities" ("Address", "ClientId", "PublicKey", "CreatedAt", "IdentityVersion", "TierIdBeforeDeletion", "TierId", "DeletionGracePeriodEndsAt", "Status") FROM stdin;
did:e:localhost:dids:0f3e40164b6c495c28674f	test	\\x0101010101	2024-11-15 09:25:40.026358+00	1	\N	TIRw9Lu4CoxXoSYDvSOI	\N	0
did:e:localhost:dids:8234cca0160ff05c785636	test	\\x0202020202	2024-11-15 09:25:40.031663+00	1	\N	TIRw9Lu4CoxXoSYDvSOI	\N	0
\.


--
-- Data for Name: IdentityDeletionProcessAuditLog; Type: TABLE DATA; Schema: Devices; Owner: postgres
--

COPY "Devices"."IdentityDeletionProcessAuditLog" ("Id", "CreatedAt", "MessageKey", "IdentityAddressHash", "DeviceIdHash", "OldStatus", "NewStatus", "IdentityDeletionProcessId", "AdditionalData") FROM stdin;
\.


--
-- Data for Name: IdentityDeletionProcesses; Type: TABLE DATA; Schema: Devices; Owner: postgres
--

COPY "Devices"."IdentityDeletionProcesses" ("Id", "Status", "DeletionStartedAt", "CreatedAt", "ApprovalReminder1SentAt", "ApprovalReminder2SentAt", "ApprovalReminder3SentAt", "ApprovedAt", "ApprovedByDevice", "RejectedAt", "RejectedByDevice", "CancelledAt", "CancelledByDevice", "GracePeriodEndsAt", "GracePeriodReminder1SentAt", "GracePeriodReminder2SentAt", "GracePeriodReminder3SentAt", "IdentityAddress") FROM stdin;
\.


--
-- Data for Name: OpenIddictApplications; Type: TABLE DATA; Schema: Devices; Owner: postgres
--

COPY "Devices"."OpenIddictApplications" ("Id", "DefaultTier", "CreatedAt", "MaxIdentities", "ApplicationType", "ClientId", "ClientSecret", "ClientType", "ConcurrencyToken", "ConsentType", "DisplayName", "DisplayNames", "JsonWebKeySet", "Permissions", "PostLogoutRedirectUris", "Properties", "RedirectUris", "Requirements", "Settings") FROM stdin;
b90d910f-2611-4e95-9d2d-3e97dcdd21ec	TIRw9Lu4CoxXoSYDvSOI	2024-11-15 09:36:34.375377+00	\N	\N	test	AQAAAAEAACcQAAAAEM2DW5MaK+7anSFiGoMS8CX2TnH/hQ3F9VHiEsSdI+kLmHF8OqjYHD/PQO/n51287g==	confidential	1b096b2b-8bee-41e8-84ac-3064065f806f	\N	test	\N	\N	["ept:token","gt:password"]	\N	\N	\N	\N	\N
\.


--
-- Data for Name: OpenIddictAuthorizations; Type: TABLE DATA; Schema: Devices; Owner: postgres
--

COPY "Devices"."OpenIddictAuthorizations" ("Id", "ApplicationId", "ConcurrencyToken", "CreationDate", "Properties", "Scopes", "Status", "Subject", "Type") FROM stdin;
\.


--
-- Data for Name: OpenIddictScopes; Type: TABLE DATA; Schema: Devices; Owner: postgres
--

COPY "Devices"."OpenIddictScopes" ("Id", "ConcurrencyToken", "Description", "Descriptions", "DisplayName", "DisplayNames", "Name", "Properties", "Resources") FROM stdin;
\.


--
-- Data for Name: OpenIddictTokens; Type: TABLE DATA; Schema: Devices; Owner: postgres
--

COPY "Devices"."OpenIddictTokens" ("Id", "ApplicationId", "AuthorizationId", "ConcurrencyToken", "CreationDate", "ExpirationDate", "Payload", "Properties", "RedemptionDate", "ReferenceId", "Status", "Subject", "Type") FROM stdin;
\.


--
-- Data for Name: PnsRegistrations; Type: TABLE DATA; Schema: Devices; Owner: postgres
--

COPY "Devices"."PnsRegistrations" ("DeviceId", "IdentityAddress", "DevicePushIdentifier", "Handle", "AppId", "UpdatedAt", "Environment") FROM stdin;
\.


--
-- Data for Name: Tiers; Type: TABLE DATA; Schema: Devices; Owner: postgres
--

COPY "Devices"."Tiers" ("Id", "Name", "CanBeUsedAsDefaultForClient", "CanBeManuallyAssigned") FROM stdin;
TIRw9Lu4CoxXoSYDvSOI	Basic	t	t
TIR00000000000000001	Queued for Deletion	f	f
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: Devices; Owner: postgres
--

COPY "Devices"."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20240701074627_Init	8.0.10
20240708114348_AddAdditionalDataToIdentityDeletionProcessAuditLogEntry	8.0.10
20240830164312_HashIndexesForIds	8.0.10
20240902141902_MakeIdentityDeletionProcessDeletionStartedAtPropertyNullable	8.0.10
20240909071633_AddUniqueIndexOnActiveDeletionProcesses	8.0.10
\.


--
-- Data for Name: FileMetadata; Type: TABLE DATA; Schema: Files; Owner: postgres
--

COPY "Files"."FileMetadata" ("Id", "CreatedAt", "CreatedBy", "CreatedByDevice", "ModifiedAt", "ModifiedBy", "ModifiedByDevice", "DeletedAt", "DeletedBy", "DeletedByDevice", "Owner", "OwnerSignature", "CipherSize", "CipherHash", "ExpiresAt", "EncryptedProperties") FROM stdin;
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: Files; Owner: postgres
--

COPY "Files"."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20240701074820_Init	8.0.10
\.


--
-- Data for Name: Attachments; Type: TABLE DATA; Schema: Messages; Owner: postgres
--

COPY "Messages"."Attachments" ("Id", "MessageId") FROM stdin;
\.


--
-- Data for Name: Messages; Type: TABLE DATA; Schema: Messages; Owner: postgres
--

COPY "Messages"."Messages" ("Id", "CreatedAt", "CreatedBy", "CreatedByDevice", "Body") FROM stdin;
\.


--
-- Data for Name: RecipientInformation; Type: TABLE DATA; Schema: Messages; Owner: postgres
--

COPY "Messages"."RecipientInformation" ("Id", "Address", "EncryptedKey", "ReceivedAt", "ReceivedByDevice", "MessageId", "IsRelationshipDecomposedByRecipient", "IsRelationshipDecomposedBySender", "RelationshipId") FROM stdin;
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: Messages; Owner: postgres
--

COPY "Messages"."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20240701075023_Init	8.0.10
20240703093047_RemoveRelationshipId	8.0.10
20240710125429_AddIsRelationshipDecomposedByRecipientAndIsRelationshipDecomposedBySenderProperties	8.0.10
20240830164612_HashIndexForMessageCreatedByField	8.0.10
20241015104418_AddRelationshipIdToRecipientInformation	8.0.10
\.


--
-- Data for Name: Identities; Type: TABLE DATA; Schema: Quotas; Owner: postgres
--

COPY "Quotas"."Identities" ("Address", "TierId") FROM stdin;
did:e:localhost:dids:8234cca0160ff05c785636	TIRw9Lu4CoxXoSYDvSOI
did:e:localhost:dids:0f3e40164b6c495c28674f	TIRw9Lu4CoxXoSYDvSOI
\.


--
-- Data for Name: IndividualQuotas; Type: TABLE DATA; Schema: Quotas; Owner: postgres
--

COPY "Quotas"."IndividualQuotas" ("Id", "ApplyTo", "MetricKey", "Max", "Period") FROM stdin;
\.


--
-- Data for Name: MetricStatuses; Type: TABLE DATA; Schema: Quotas; Owner: postgres
--

COPY "Quotas"."MetricStatuses" ("MetricKey", "Owner", "IsExhaustedUntil") FROM stdin;
\.


--
-- Data for Name: TierQuotaDefinitions; Type: TABLE DATA; Schema: Quotas; Owner: postgres
--

COPY "Quotas"."TierQuotaDefinitions" ("Id", "MetricKey", "Max", "Period", "TierId") FROM stdin;
TQDaPdRb2W22vob3AkBb	NumberOfRelationships	0	5	TIR00000000000000001
TQDbUrtZnWKsqH3XhMRX	NumberOfSentMessages	0	5	TIR00000000000000001
TQDEgWIj5kfpeEaCqVpH	NumberOfTokens	0	5	TIR00000000000000001
TQDjlUNatUCFC4D3Eyap	NumberOfRelationshipTemplates	0	5	TIR00000000000000001
TQDkHLMEsDvYwKjJidhq	UsedFileStorageSpace	0	5	TIR00000000000000001
TQDnKDVG6QBv0fyzczx2	NumberOfCreatedDevices	0	5	TIR00000000000000001
TQDNSQ7QwkdqhyoFw2se	NumberOfFiles	0	5	TIR00000000000000001
TQDweFsrLraXPM6gKigI	NumberOfCreatedChallenges	0	5	TIR00000000000000001
\.


--
-- Data for Name: TierQuotas; Type: TABLE DATA; Schema: Quotas; Owner: postgres
--

COPY "Quotas"."TierQuotas" ("Id", "DefinitionId", "ApplyTo") FROM stdin;
\.


--
-- Data for Name: Tiers; Type: TABLE DATA; Schema: Quotas; Owner: postgres
--

COPY "Quotas"."Tiers" ("Id", "Name") FROM stdin;
TIRw9Lu4CoxXoSYDvSOI	Basic
TIR00000000000000001	Queued for Deletion
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: Quotas; Owner: postgres
--

COPY "Quotas"."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20240701075245_Init	8.0.10
\.


--
-- Data for Name: RelationshipAuditLog; Type: TABLE DATA; Schema: Relationships; Owner: postgres
--

COPY "Relationships"."RelationshipAuditLog" ("Id", "Reason", "OldStatus", "NewStatus", "CreatedBy", "CreatedByDevice", "CreatedAt", "RelationshipId") FROM stdin;
\.


--
-- Data for Name: RelationshipTemplateAllocations; Type: TABLE DATA; Schema: Relationships; Owner: postgres
--

COPY "Relationships"."RelationshipTemplateAllocations" ("Id", "RelationshipTemplateId", "AllocatedBy", "AllocatedAt", "AllocatedByDevice") FROM stdin;
\.


--
-- Data for Name: RelationshipTemplates; Type: TABLE DATA; Schema: Relationships; Owner: postgres
--

COPY "Relationships"."RelationshipTemplates" ("Id", "CreatedBy", "CreatedByDevice", "MaxNumberOfAllocations", "ExpiresAt", "Content", "CreatedAt", "ForIdentity", "Password") FROM stdin;
\.


--
-- Data for Name: Relationships; Type: TABLE DATA; Schema: Relationships; Owner: postgres
--

COPY "Relationships"."Relationships" ("Id", "RelationshipTemplateId", "From", "To", "CreatedAt", "Status", "CreationContent", "CreationResponseContent", "FromHasDecomposed", "ToHasDecomposed") FROM stdin;
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: Relationships; Owner: postgres
--

COPY "Relationships"."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20240701075857_Init	8.0.10
20240703100000_ConfigureRelationshipAuditLogDeleteBehavior	8.0.10
20240830113359_RemoveDeletedAtPropertyFromRelationshipTemplate	8.0.10
20240830164658_HashIndexesForRelationshipIdentityAddresses	8.0.10
20240906075221_PersonalizedRelationshipTemplates	8.0.10
20241011081142_AddPasswordToRelationshipTemplate	8.0.10
20241106092429_MakeTemplateNullableInRelationship	8.0.10
\.


--
-- Data for Name: DatawalletModifications; Type: TABLE DATA; Schema: Synchronization; Owner: postgres
--

COPY "Synchronization"."DatawalletModifications" ("Id", "DatawalletId", "DatawalletVersion", "Index", "ObjectIdentifier", "PayloadCategory", "CreatedAt", "CreatedBy", "CreatedByDevice", "Collection", "Type", "EncryptedPayload") FROM stdin;
\.


--
-- Data for Name: Datawallets; Type: TABLE DATA; Schema: Synchronization; Owner: postgres
--

COPY "Synchronization"."Datawallets" ("Id", "Owner", "Version") FROM stdin;
\.


--
-- Data for Name: ExternalEvents; Type: TABLE DATA; Schema: Synchronization; Owner: postgres
--

COPY "Synchronization"."ExternalEvents" ("Id", "Type", "Index", "Owner", "CreatedAt", "Payload", "SyncErrorCount", "SyncRunId", "Context", "IsDeliveryBlocked") FROM stdin;
\.


--
-- Data for Name: SyncErrors; Type: TABLE DATA; Schema: Synchronization; Owner: postgres
--

COPY "Synchronization"."SyncErrors" ("Id", "SyncRunId", "ExternalEventId", "ErrorCode") FROM stdin;
\.


--
-- Data for Name: SyncRuns; Type: TABLE DATA; Schema: Synchronization; Owner: postgres
--

COPY "Synchronization"."SyncRuns" ("Id", "Type", "ExpiresAt", "Index", "CreatedAt", "CreatedBy", "CreatedByDevice", "FinalizedAt", "EventCount") FROM stdin;
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: Synchronization; Owner: postgres
--

COPY "Synchronization"."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20240701081741_Init	8.0.10
20240830112624_RemoveBlobReferenceColumn	8.0.10
20240904100328_IncreaseMaxSizeOfSyncErrorErrorCodeTo100	8.0.10
20241016072722_AddIsDeliveryBlockedAndContextColumnsToExternalEventsTable	8.0.10
\.


--
-- Data for Name: Tokens; Type: TABLE DATA; Schema: Tokens; Owner: postgres
--

COPY "Tokens"."Tokens" ("Id", "CreatedBy", "CreatedByDevice", "Content", "CreatedAt", "ExpiresAt", "ForIdentity", "Password") FROM stdin;
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: Tokens; Owner: postgres
--

COPY "Tokens"."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20240701082135_Init	8.0.10
20240807121033_PersonalizedTokens	8.0.10
20241011123029_AddPasswordToToken	8.0.10
\.


--
-- Name: AspNetRoleClaims_Id_seq; Type: SEQUENCE SET; Schema: Devices; Owner: postgres
--

SELECT pg_catalog.setval('"Devices"."AspNetRoleClaims_Id_seq"', 1, false);


--
-- Name: AspNetUserClaims_Id_seq; Type: SEQUENCE SET; Schema: Devices; Owner: postgres
--

SELECT pg_catalog.setval('"Devices"."AspNetUserClaims_Id_seq"', 1, false);


--
-- Name: RecipientInformation_Id_seq; Type: SEQUENCE SET; Schema: Messages; Owner: postgres
--

SELECT pg_catalog.setval('"Messages"."RecipientInformation_Id_seq"', 1, false);


--
-- Name: RelationshipTemplateAllocations_Id_seq; Type: SEQUENCE SET; Schema: Relationships; Owner: postgres
--

SELECT pg_catalog.setval('"Relationships"."RelationshipTemplateAllocations_Id_seq"', 1, false);


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: AdminUi; Owner: postgres
--

ALTER TABLE ONLY "AdminUi"."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: Challenges PK_Challenges; Type: CONSTRAINT; Schema: Challenges; Owner: postgres
--

ALTER TABLE ONLY "Challenges"."Challenges"
    ADD CONSTRAINT "PK_Challenges" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: Challenges; Owner: postgres
--

ALTER TABLE ONLY "Challenges"."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: AspNetRoleClaims PK_AspNetRoleClaims; Type: CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."AspNetRoleClaims"
    ADD CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id");


--
-- Name: AspNetRoles PK_AspNetRoles; Type: CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."AspNetRoles"
    ADD CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id");


--
-- Name: AspNetUserClaims PK_AspNetUserClaims; Type: CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."AspNetUserClaims"
    ADD CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id");


--
-- Name: AspNetUserLogins PK_AspNetUserLogins; Type: CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."AspNetUserLogins"
    ADD CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey");


--
-- Name: AspNetUserRoles PK_AspNetUserRoles; Type: CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."AspNetUserRoles"
    ADD CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId");


--
-- Name: AspNetUserTokens PK_AspNetUserTokens; Type: CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."AspNetUserTokens"
    ADD CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name");


--
-- Name: AspNetUsers PK_AspNetUsers; Type: CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."AspNetUsers"
    ADD CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id");


--
-- Name: Devices PK_Devices; Type: CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."Devices"
    ADD CONSTRAINT "PK_Devices" PRIMARY KEY ("Id");


--
-- Name: Identities PK_Identities; Type: CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."Identities"
    ADD CONSTRAINT "PK_Identities" PRIMARY KEY ("Address");


--
-- Name: IdentityDeletionProcessAuditLog PK_IdentityDeletionProcessAuditLog; Type: CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."IdentityDeletionProcessAuditLog"
    ADD CONSTRAINT "PK_IdentityDeletionProcessAuditLog" PRIMARY KEY ("Id");


--
-- Name: IdentityDeletionProcesses PK_IdentityDeletionProcesses; Type: CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."IdentityDeletionProcesses"
    ADD CONSTRAINT "PK_IdentityDeletionProcesses" PRIMARY KEY ("Id");


--
-- Name: OpenIddictApplications PK_OpenIddictApplications; Type: CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."OpenIddictApplications"
    ADD CONSTRAINT "PK_OpenIddictApplications" PRIMARY KEY ("Id");


--
-- Name: OpenIddictAuthorizations PK_OpenIddictAuthorizations; Type: CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."OpenIddictAuthorizations"
    ADD CONSTRAINT "PK_OpenIddictAuthorizations" PRIMARY KEY ("Id");


--
-- Name: OpenIddictScopes PK_OpenIddictScopes; Type: CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."OpenIddictScopes"
    ADD CONSTRAINT "PK_OpenIddictScopes" PRIMARY KEY ("Id");


--
-- Name: OpenIddictTokens PK_OpenIddictTokens; Type: CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."OpenIddictTokens"
    ADD CONSTRAINT "PK_OpenIddictTokens" PRIMARY KEY ("Id");


--
-- Name: PnsRegistrations PK_PnsRegistrations; Type: CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."PnsRegistrations"
    ADD CONSTRAINT "PK_PnsRegistrations" PRIMARY KEY ("DeviceId");


--
-- Name: Tiers PK_Tiers; Type: CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."Tiers"
    ADD CONSTRAINT "PK_Tiers" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: FileMetadata PK_FileMetadata; Type: CONSTRAINT; Schema: Files; Owner: postgres
--

ALTER TABLE ONLY "Files"."FileMetadata"
    ADD CONSTRAINT "PK_FileMetadata" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: Files; Owner: postgres
--

ALTER TABLE ONLY "Files"."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: Attachments PK_Attachments; Type: CONSTRAINT; Schema: Messages; Owner: postgres
--

ALTER TABLE ONLY "Messages"."Attachments"
    ADD CONSTRAINT "PK_Attachments" PRIMARY KEY ("Id", "MessageId");


--
-- Name: Messages PK_Messages; Type: CONSTRAINT; Schema: Messages; Owner: postgres
--

ALTER TABLE ONLY "Messages"."Messages"
    ADD CONSTRAINT "PK_Messages" PRIMARY KEY ("Id");


--
-- Name: RecipientInformation PK_RecipientInformation; Type: CONSTRAINT; Schema: Messages; Owner: postgres
--

ALTER TABLE ONLY "Messages"."RecipientInformation"
    ADD CONSTRAINT "PK_RecipientInformation" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: Messages; Owner: postgres
--

ALTER TABLE ONLY "Messages"."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: Identities PK_Identities; Type: CONSTRAINT; Schema: Quotas; Owner: postgres
--

ALTER TABLE ONLY "Quotas"."Identities"
    ADD CONSTRAINT "PK_Identities" PRIMARY KEY ("Address");


--
-- Name: IndividualQuotas PK_IndividualQuotas; Type: CONSTRAINT; Schema: Quotas; Owner: postgres
--

ALTER TABLE ONLY "Quotas"."IndividualQuotas"
    ADD CONSTRAINT "PK_IndividualQuotas" PRIMARY KEY ("Id");


--
-- Name: MetricStatuses PK_MetricStatuses; Type: CONSTRAINT; Schema: Quotas; Owner: postgres
--

ALTER TABLE ONLY "Quotas"."MetricStatuses"
    ADD CONSTRAINT "PK_MetricStatuses" PRIMARY KEY ("Owner", "MetricKey");


--
-- Name: TierQuotaDefinitions PK_TierQuotaDefinitions; Type: CONSTRAINT; Schema: Quotas; Owner: postgres
--

ALTER TABLE ONLY "Quotas"."TierQuotaDefinitions"
    ADD CONSTRAINT "PK_TierQuotaDefinitions" PRIMARY KEY ("Id");


--
-- Name: TierQuotas PK_TierQuotas; Type: CONSTRAINT; Schema: Quotas; Owner: postgres
--

ALTER TABLE ONLY "Quotas"."TierQuotas"
    ADD CONSTRAINT "PK_TierQuotas" PRIMARY KEY ("Id");


--
-- Name: Tiers PK_Tiers; Type: CONSTRAINT; Schema: Quotas; Owner: postgres
--

ALTER TABLE ONLY "Quotas"."Tiers"
    ADD CONSTRAINT "PK_Tiers" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: Quotas; Owner: postgres
--

ALTER TABLE ONLY "Quotas"."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: RelationshipAuditLog PK_RelationshipAuditLog; Type: CONSTRAINT; Schema: Relationships; Owner: postgres
--

ALTER TABLE ONLY "Relationships"."RelationshipAuditLog"
    ADD CONSTRAINT "PK_RelationshipAuditLog" PRIMARY KEY ("Id");


--
-- Name: RelationshipTemplateAllocations PK_RelationshipTemplateAllocations; Type: CONSTRAINT; Schema: Relationships; Owner: postgres
--

ALTER TABLE ONLY "Relationships"."RelationshipTemplateAllocations"
    ADD CONSTRAINT "PK_RelationshipTemplateAllocations" PRIMARY KEY ("Id");


--
-- Name: RelationshipTemplates PK_RelationshipTemplates; Type: CONSTRAINT; Schema: Relationships; Owner: postgres
--

ALTER TABLE ONLY "Relationships"."RelationshipTemplates"
    ADD CONSTRAINT "PK_RelationshipTemplates" PRIMARY KEY ("Id");


--
-- Name: Relationships PK_Relationships; Type: CONSTRAINT; Schema: Relationships; Owner: postgres
--

ALTER TABLE ONLY "Relationships"."Relationships"
    ADD CONSTRAINT "PK_Relationships" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: Relationships; Owner: postgres
--

ALTER TABLE ONLY "Relationships"."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: DatawalletModifications PK_DatawalletModifications; Type: CONSTRAINT; Schema: Synchronization; Owner: postgres
--

ALTER TABLE ONLY "Synchronization"."DatawalletModifications"
    ADD CONSTRAINT "PK_DatawalletModifications" PRIMARY KEY ("Id");


--
-- Name: Datawallets PK_Datawallets; Type: CONSTRAINT; Schema: Synchronization; Owner: postgres
--

ALTER TABLE ONLY "Synchronization"."Datawallets"
    ADD CONSTRAINT "PK_Datawallets" PRIMARY KEY ("Id");


--
-- Name: ExternalEvents PK_ExternalEvents; Type: CONSTRAINT; Schema: Synchronization; Owner: postgres
--

ALTER TABLE ONLY "Synchronization"."ExternalEvents"
    ADD CONSTRAINT "PK_ExternalEvents" PRIMARY KEY ("Id");


--
-- Name: SyncErrors PK_SyncErrors; Type: CONSTRAINT; Schema: Synchronization; Owner: postgres
--

ALTER TABLE ONLY "Synchronization"."SyncErrors"
    ADD CONSTRAINT "PK_SyncErrors" PRIMARY KEY ("Id");


--
-- Name: SyncRuns PK_SyncRuns; Type: CONSTRAINT; Schema: Synchronization; Owner: postgres
--

ALTER TABLE ONLY "Synchronization"."SyncRuns"
    ADD CONSTRAINT "PK_SyncRuns" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: Synchronization; Owner: postgres
--

ALTER TABLE ONLY "Synchronization"."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: Tokens PK_Tokens; Type: CONSTRAINT; Schema: Tokens; Owner: postgres
--

ALTER TABLE ONLY "Tokens"."Tokens"
    ADD CONSTRAINT "PK_Tokens" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: Tokens; Owner: postgres
--

ALTER TABLE ONLY "Tokens"."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: IX_AspNetRoleClaims_RoleId; Type: INDEX; Schema: Devices; Owner: postgres
--

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "Devices"."AspNetRoleClaims" USING btree ("RoleId");


--
-- Name: IX_AspNetUserClaims_UserId; Type: INDEX; Schema: Devices; Owner: postgres
--

CREATE INDEX "IX_AspNetUserClaims_UserId" ON "Devices"."AspNetUserClaims" USING btree ("UserId");


--
-- Name: IX_AspNetUserLogins_UserId; Type: INDEX; Schema: Devices; Owner: postgres
--

CREATE INDEX "IX_AspNetUserLogins_UserId" ON "Devices"."AspNetUserLogins" USING btree ("UserId");


--
-- Name: IX_AspNetUserRoles_RoleId; Type: INDEX; Schema: Devices; Owner: postgres
--

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "Devices"."AspNetUserRoles" USING btree ("RoleId");


--
-- Name: IX_AspNetUsers_DeviceId; Type: INDEX; Schema: Devices; Owner: postgres
--

CREATE UNIQUE INDEX "IX_AspNetUsers_DeviceId" ON "Devices"."AspNetUsers" USING btree ("DeviceId");


--
-- Name: IX_Devices_IdentityAddress; Type: INDEX; Schema: Devices; Owner: postgres
--

CREATE INDEX "IX_Devices_IdentityAddress" ON "Devices"."Devices" USING btree ("IdentityAddress");


--
-- Name: IX_Identities_ClientId; Type: INDEX; Schema: Devices; Owner: postgres
--

CREATE INDEX "IX_Identities_ClientId" ON "Devices"."Identities" USING hash ("ClientId");


--
-- Name: IX_Identities_TierId; Type: INDEX; Schema: Devices; Owner: postgres
--

CREATE INDEX "IX_Identities_TierId" ON "Devices"."Identities" USING hash ("TierId");


--
-- Name: IX_IdentityDeletionProcessAuditLog_IdentityDeletionProcessId; Type: INDEX; Schema: Devices; Owner: postgres
--

CREATE INDEX "IX_IdentityDeletionProcessAuditLog_IdentityDeletionProcessId" ON "Devices"."IdentityDeletionProcessAuditLog" USING btree ("IdentityDeletionProcessId");


--
-- Name: IX_IdentityDeletionProcesses_IdentityAddress; Type: INDEX; Schema: Devices; Owner: postgres
--

CREATE INDEX "IX_IdentityDeletionProcesses_IdentityAddress" ON "Devices"."IdentityDeletionProcesses" USING btree ("IdentityAddress");


--
-- Name: IX_OpenIddictApplications_ClientId; Type: INDEX; Schema: Devices; Owner: postgres
--

CREATE UNIQUE INDEX "IX_OpenIddictApplications_ClientId" ON "Devices"."OpenIddictApplications" USING btree ("ClientId");


--
-- Name: IX_OpenIddictApplications_DefaultTier; Type: INDEX; Schema: Devices; Owner: postgres
--

CREATE INDEX "IX_OpenIddictApplications_DefaultTier" ON "Devices"."OpenIddictApplications" USING btree ("DefaultTier");


--
-- Name: IX_OpenIddictAuthorizations_ApplicationId_Status_Subject_Type; Type: INDEX; Schema: Devices; Owner: postgres
--

CREATE INDEX "IX_OpenIddictAuthorizations_ApplicationId_Status_Subject_Type" ON "Devices"."OpenIddictAuthorizations" USING btree ("ApplicationId", "Status", "Subject", "Type");


--
-- Name: IX_OpenIddictScopes_Name; Type: INDEX; Schema: Devices; Owner: postgres
--

CREATE UNIQUE INDEX "IX_OpenIddictScopes_Name" ON "Devices"."OpenIddictScopes" USING btree ("Name");


--
-- Name: IX_OpenIddictTokens_ApplicationId_Status_Subject_Type; Type: INDEX; Schema: Devices; Owner: postgres
--

CREATE INDEX "IX_OpenIddictTokens_ApplicationId_Status_Subject_Type" ON "Devices"."OpenIddictTokens" USING btree ("ApplicationId", "Status", "Subject", "Type");


--
-- Name: IX_OpenIddictTokens_AuthorizationId; Type: INDEX; Schema: Devices; Owner: postgres
--

CREATE INDEX "IX_OpenIddictTokens_AuthorizationId" ON "Devices"."OpenIddictTokens" USING btree ("AuthorizationId");


--
-- Name: IX_OpenIddictTokens_ReferenceId; Type: INDEX; Schema: Devices; Owner: postgres
--

CREATE UNIQUE INDEX "IX_OpenIddictTokens_ReferenceId" ON "Devices"."OpenIddictTokens" USING btree ("ReferenceId");


--
-- Name: IX_Tiers_Name; Type: INDEX; Schema: Devices; Owner: postgres
--

CREATE UNIQUE INDEX "IX_Tiers_Name" ON "Devices"."Tiers" USING btree ("Name");


--
-- Name: IX_only_one_active_deletion_process; Type: INDEX; Schema: Devices; Owner: postgres
--

CREATE UNIQUE INDEX "IX_only_one_active_deletion_process" ON "Devices"."IdentityDeletionProcesses" USING btree ("IdentityAddress") WHERE ("Status" = 1);


--
-- Name: RoleNameIndex; Type: INDEX; Schema: Devices; Owner: postgres
--

CREATE UNIQUE INDEX "RoleNameIndex" ON "Devices"."AspNetRoles" USING btree ("NormalizedName");


--
-- Name: UserNameIndex; Type: INDEX; Schema: Devices; Owner: postgres
--

CREATE UNIQUE INDEX "UserNameIndex" ON "Devices"."AspNetUsers" USING btree ("NormalizedUserName");


--
-- Name: IX_Attachments_MessageId; Type: INDEX; Schema: Messages; Owner: postgres
--

CREATE INDEX "IX_Attachments_MessageId" ON "Messages"."Attachments" USING btree ("MessageId");


--
-- Name: IX_Messages_CreatedBy; Type: INDEX; Schema: Messages; Owner: postgres
--

CREATE INDEX "IX_Messages_CreatedBy" ON "Messages"."Messages" USING hash ("CreatedBy");


--
-- Name: IX_RecipientInformation_Address_MessageId; Type: INDEX; Schema: Messages; Owner: postgres
--

CREATE INDEX "IX_RecipientInformation_Address_MessageId" ON "Messages"."RecipientInformation" USING btree ("Address", "MessageId");


--
-- Name: IX_RecipientInformation_MessageId; Type: INDEX; Schema: Messages; Owner: postgres
--

CREATE INDEX "IX_RecipientInformation_MessageId" ON "Messages"."RecipientInformation" USING btree ("MessageId");


--
-- Name: IX_RecipientInformation_ReceivedAt; Type: INDEX; Schema: Messages; Owner: postgres
--

CREATE INDEX "IX_RecipientInformation_ReceivedAt" ON "Messages"."RecipientInformation" USING btree ("ReceivedAt");


--
-- Name: IX_Identities_TierId; Type: INDEX; Schema: Quotas; Owner: postgres
--

CREATE INDEX "IX_Identities_TierId" ON "Quotas"."Identities" USING btree ("TierId");


--
-- Name: IX_IndividualQuotas_ApplyTo; Type: INDEX; Schema: Quotas; Owner: postgres
--

CREATE INDEX "IX_IndividualQuotas_ApplyTo" ON "Quotas"."IndividualQuotas" USING btree ("ApplyTo");


--
-- Name: IX_MetricStatuses_MetricKey; Type: INDEX; Schema: Quotas; Owner: postgres
--

CREATE INDEX "IX_MetricStatuses_MetricKey" ON "Quotas"."MetricStatuses" USING btree ("MetricKey") INCLUDE ("IsExhaustedUntil");


--
-- Name: IX_TierQuotaDefinitions_TierId; Type: INDEX; Schema: Quotas; Owner: postgres
--

CREATE INDEX "IX_TierQuotaDefinitions_TierId" ON "Quotas"."TierQuotaDefinitions" USING btree ("TierId");


--
-- Name: IX_TierQuotas_ApplyTo; Type: INDEX; Schema: Quotas; Owner: postgres
--

CREATE INDEX "IX_TierQuotas_ApplyTo" ON "Quotas"."TierQuotas" USING btree ("ApplyTo");


--
-- Name: IX_TierQuotas_DefinitionId; Type: INDEX; Schema: Quotas; Owner: postgres
--

CREATE INDEX "IX_TierQuotas_DefinitionId" ON "Quotas"."TierQuotas" USING btree ("DefinitionId");


--
-- Name: IX_RelationshipAuditLog_RelationshipId; Type: INDEX; Schema: Relationships; Owner: postgres
--

CREATE INDEX "IX_RelationshipAuditLog_RelationshipId" ON "Relationships"."RelationshipAuditLog" USING btree ("RelationshipId");


--
-- Name: IX_RelationshipTemplateAllocations_RelationshipTemplateId_Allo~; Type: INDEX; Schema: Relationships; Owner: postgres
--

CREATE INDEX "IX_RelationshipTemplateAllocations_RelationshipTemplateId_Allo~" ON "Relationships"."RelationshipTemplateAllocations" USING btree ("RelationshipTemplateId", "AllocatedBy");


--
-- Name: IX_Relationships_From; Type: INDEX; Schema: Relationships; Owner: postgres
--

CREATE INDEX "IX_Relationships_From" ON "Relationships"."Relationships" USING hash ("From");


--
-- Name: IX_Relationships_RelationshipTemplateId; Type: INDEX; Schema: Relationships; Owner: postgres
--

CREATE INDEX "IX_Relationships_RelationshipTemplateId" ON "Relationships"."Relationships" USING btree ("RelationshipTemplateId");


--
-- Name: IX_Relationships_To; Type: INDEX; Schema: Relationships; Owner: postgres
--

CREATE INDEX "IX_Relationships_To" ON "Relationships"."Relationships" USING hash ("To");


--
-- Name: IX_DatawalletModifications_CreatedBy_Index; Type: INDEX; Schema: Synchronization; Owner: postgres
--

CREATE UNIQUE INDEX "IX_DatawalletModifications_CreatedBy_Index" ON "Synchronization"."DatawalletModifications" USING btree ("CreatedBy", "Index");


--
-- Name: IX_DatawalletModifications_DatawalletId; Type: INDEX; Schema: Synchronization; Owner: postgres
--

CREATE INDEX "IX_DatawalletModifications_DatawalletId" ON "Synchronization"."DatawalletModifications" USING btree ("DatawalletId");


--
-- Name: IX_Datawallets_Owner; Type: INDEX; Schema: Synchronization; Owner: postgres
--

CREATE UNIQUE INDEX "IX_Datawallets_Owner" ON "Synchronization"."Datawallets" USING btree ("Owner");


--
-- Name: IX_ExternalEvents_Owner_Index; Type: INDEX; Schema: Synchronization; Owner: postgres
--

CREATE UNIQUE INDEX "IX_ExternalEvents_Owner_Index" ON "Synchronization"."ExternalEvents" USING btree ("Owner", "Index");


--
-- Name: IX_ExternalEvents_Owner_SyncRunId; Type: INDEX; Schema: Synchronization; Owner: postgres
--

CREATE INDEX "IX_ExternalEvents_Owner_SyncRunId" ON "Synchronization"."ExternalEvents" USING btree ("Owner", "SyncRunId");


--
-- Name: IX_ExternalEvents_SyncRunId; Type: INDEX; Schema: Synchronization; Owner: postgres
--

CREATE INDEX "IX_ExternalEvents_SyncRunId" ON "Synchronization"."ExternalEvents" USING btree ("SyncRunId");


--
-- Name: IX_SyncErrors_ExternalEventId; Type: INDEX; Schema: Synchronization; Owner: postgres
--

CREATE INDEX "IX_SyncErrors_ExternalEventId" ON "Synchronization"."SyncErrors" USING btree ("ExternalEventId");


--
-- Name: IX_SyncErrors_SyncRunId_ExternalEventId; Type: INDEX; Schema: Synchronization; Owner: postgres
--

CREATE UNIQUE INDEX "IX_SyncErrors_SyncRunId_ExternalEventId" ON "Synchronization"."SyncErrors" USING btree ("SyncRunId", "ExternalEventId");


--
-- Name: IX_SyncRuns_CreatedBy_FinalizedAt; Type: INDEX; Schema: Synchronization; Owner: postgres
--

CREATE INDEX "IX_SyncRuns_CreatedBy_FinalizedAt" ON "Synchronization"."SyncRuns" USING btree ("CreatedBy", "FinalizedAt");


--
-- Name: IX_SyncRuns_CreatedBy_Index; Type: INDEX; Schema: Synchronization; Owner: postgres
--

CREATE UNIQUE INDEX "IX_SyncRuns_CreatedBy_Index" ON "Synchronization"."SyncRuns" USING btree ("CreatedBy", "Index");


--
-- Name: AspNetRoleClaims FK_AspNetRoleClaims_AspNetRoles_RoleId; Type: FK CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."AspNetRoleClaims"
    ADD CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "Devices"."AspNetRoles"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserClaims FK_AspNetUserClaims_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."AspNetUserClaims"
    ADD CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "Devices"."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserLogins FK_AspNetUserLogins_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."AspNetUserLogins"
    ADD CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "Devices"."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserRoles FK_AspNetUserRoles_AspNetRoles_RoleId; Type: FK CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "Devices"."AspNetRoles"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserRoles FK_AspNetUserRoles_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "Devices"."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserTokens FK_AspNetUserTokens_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."AspNetUserTokens"
    ADD CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "Devices"."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUsers FK_AspNetUsers_Devices_DeviceId; Type: FK CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."AspNetUsers"
    ADD CONSTRAINT "FK_AspNetUsers_Devices_DeviceId" FOREIGN KEY ("DeviceId") REFERENCES "Devices"."Devices"("Id") ON DELETE CASCADE;


--
-- Name: Devices FK_Devices_Identities_IdentityAddress; Type: FK CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."Devices"
    ADD CONSTRAINT "FK_Devices_Identities_IdentityAddress" FOREIGN KEY ("IdentityAddress") REFERENCES "Devices"."Identities"("Address") ON DELETE CASCADE;


--
-- Name: IdentityDeletionProcessAuditLog FK_IdentityDeletionProcessAuditLog_IdentityDeletionProcesses_I~; Type: FK CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."IdentityDeletionProcessAuditLog"
    ADD CONSTRAINT "FK_IdentityDeletionProcessAuditLog_IdentityDeletionProcesses_I~" FOREIGN KEY ("IdentityDeletionProcessId") REFERENCES "Devices"."IdentityDeletionProcesses"("Id") ON DELETE SET NULL;


--
-- Name: IdentityDeletionProcesses FK_IdentityDeletionProcesses_Identities_IdentityAddress; Type: FK CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."IdentityDeletionProcesses"
    ADD CONSTRAINT "FK_IdentityDeletionProcesses_Identities_IdentityAddress" FOREIGN KEY ("IdentityAddress") REFERENCES "Devices"."Identities"("Address") ON DELETE CASCADE;


--
-- Name: OpenIddictApplications FK_OpenIddictApplications_Tiers_DefaultTier; Type: FK CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."OpenIddictApplications"
    ADD CONSTRAINT "FK_OpenIddictApplications_Tiers_DefaultTier" FOREIGN KEY ("DefaultTier") REFERENCES "Devices"."Tiers"("Id") ON DELETE RESTRICT;


--
-- Name: OpenIddictAuthorizations FK_OpenIddictAuthorizations_OpenIddictApplications_Application~; Type: FK CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."OpenIddictAuthorizations"
    ADD CONSTRAINT "FK_OpenIddictAuthorizations_OpenIddictApplications_Application~" FOREIGN KEY ("ApplicationId") REFERENCES "Devices"."OpenIddictApplications"("Id");


--
-- Name: OpenIddictTokens FK_OpenIddictTokens_OpenIddictApplications_ApplicationId; Type: FK CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."OpenIddictTokens"
    ADD CONSTRAINT "FK_OpenIddictTokens_OpenIddictApplications_ApplicationId" FOREIGN KEY ("ApplicationId") REFERENCES "Devices"."OpenIddictApplications"("Id");


--
-- Name: OpenIddictTokens FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId; Type: FK CONSTRAINT; Schema: Devices; Owner: postgres
--

ALTER TABLE ONLY "Devices"."OpenIddictTokens"
    ADD CONSTRAINT "FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId" FOREIGN KEY ("AuthorizationId") REFERENCES "Devices"."OpenIddictAuthorizations"("Id");


--
-- Name: Attachments FK_Attachments_Messages_MessageId; Type: FK CONSTRAINT; Schema: Messages; Owner: postgres
--

ALTER TABLE ONLY "Messages"."Attachments"
    ADD CONSTRAINT "FK_Attachments_Messages_MessageId" FOREIGN KEY ("MessageId") REFERENCES "Messages"."Messages"("Id") ON DELETE CASCADE;


--
-- Name: RecipientInformation FK_RecipientInformation_Messages_MessageId; Type: FK CONSTRAINT; Schema: Messages; Owner: postgres
--

ALTER TABLE ONLY "Messages"."RecipientInformation"
    ADD CONSTRAINT "FK_RecipientInformation_Messages_MessageId" FOREIGN KEY ("MessageId") REFERENCES "Messages"."Messages"("Id") ON DELETE CASCADE;


--
-- Name: Identities FK_Identities_Tiers_TierId; Type: FK CONSTRAINT; Schema: Quotas; Owner: postgres
--

ALTER TABLE ONLY "Quotas"."Identities"
    ADD CONSTRAINT "FK_Identities_Tiers_TierId" FOREIGN KEY ("TierId") REFERENCES "Quotas"."Tiers"("Id");


--
-- Name: IndividualQuotas FK_IndividualQuotas_Identities_ApplyTo; Type: FK CONSTRAINT; Schema: Quotas; Owner: postgres
--

ALTER TABLE ONLY "Quotas"."IndividualQuotas"
    ADD CONSTRAINT "FK_IndividualQuotas_Identities_ApplyTo" FOREIGN KEY ("ApplyTo") REFERENCES "Quotas"."Identities"("Address") ON DELETE CASCADE;


--
-- Name: MetricStatuses FK_MetricStatuses_Identities_Owner; Type: FK CONSTRAINT; Schema: Quotas; Owner: postgres
--

ALTER TABLE ONLY "Quotas"."MetricStatuses"
    ADD CONSTRAINT "FK_MetricStatuses_Identities_Owner" FOREIGN KEY ("Owner") REFERENCES "Quotas"."Identities"("Address") ON DELETE CASCADE;


--
-- Name: TierQuotaDefinitions FK_TierQuotaDefinitions_Tiers_TierId; Type: FK CONSTRAINT; Schema: Quotas; Owner: postgres
--

ALTER TABLE ONLY "Quotas"."TierQuotaDefinitions"
    ADD CONSTRAINT "FK_TierQuotaDefinitions_Tiers_TierId" FOREIGN KEY ("TierId") REFERENCES "Quotas"."Tiers"("Id") ON DELETE CASCADE;


--
-- Name: TierQuotas FK_TierQuotas_Identities_ApplyTo; Type: FK CONSTRAINT; Schema: Quotas; Owner: postgres
--

ALTER TABLE ONLY "Quotas"."TierQuotas"
    ADD CONSTRAINT "FK_TierQuotas_Identities_ApplyTo" FOREIGN KEY ("ApplyTo") REFERENCES "Quotas"."Identities"("Address") ON DELETE CASCADE;


--
-- Name: TierQuotas FK_TierQuotas_TierQuotaDefinitions_DefinitionId; Type: FK CONSTRAINT; Schema: Quotas; Owner: postgres
--

ALTER TABLE ONLY "Quotas"."TierQuotas"
    ADD CONSTRAINT "FK_TierQuotas_TierQuotaDefinitions_DefinitionId" FOREIGN KEY ("DefinitionId") REFERENCES "Quotas"."TierQuotaDefinitions"("Id") ON DELETE CASCADE;


--
-- Name: RelationshipAuditLog FK_RelationshipAuditLog_Relationships_RelationshipId; Type: FK CONSTRAINT; Schema: Relationships; Owner: postgres
--

ALTER TABLE ONLY "Relationships"."RelationshipAuditLog"
    ADD CONSTRAINT "FK_RelationshipAuditLog_Relationships_RelationshipId" FOREIGN KEY ("RelationshipId") REFERENCES "Relationships"."Relationships"("Id") ON DELETE CASCADE;


--
-- Name: RelationshipTemplateAllocations FK_RelationshipTemplateAllocations_RelationshipTemplates_Relat~; Type: FK CONSTRAINT; Schema: Relationships; Owner: postgres
--

ALTER TABLE ONLY "Relationships"."RelationshipTemplateAllocations"
    ADD CONSTRAINT "FK_RelationshipTemplateAllocations_RelationshipTemplates_Relat~" FOREIGN KEY ("RelationshipTemplateId") REFERENCES "Relationships"."RelationshipTemplates"("Id") ON DELETE CASCADE;


--
-- Name: Relationships FK_Relationships_RelationshipTemplates_RelationshipTemplateId; Type: FK CONSTRAINT; Schema: Relationships; Owner: postgres
--

ALTER TABLE ONLY "Relationships"."Relationships"
    ADD CONSTRAINT "FK_Relationships_RelationshipTemplates_RelationshipTemplateId" FOREIGN KEY ("RelationshipTemplateId") REFERENCES "Relationships"."RelationshipTemplates"("Id") ON DELETE SET NULL;


--
-- Name: DatawalletModifications FK_DatawalletModifications_Datawallets_DatawalletId; Type: FK CONSTRAINT; Schema: Synchronization; Owner: postgres
--

ALTER TABLE ONLY "Synchronization"."DatawalletModifications"
    ADD CONSTRAINT "FK_DatawalletModifications_Datawallets_DatawalletId" FOREIGN KEY ("DatawalletId") REFERENCES "Synchronization"."Datawallets"("Id") ON DELETE CASCADE;


--
-- Name: ExternalEvents FK_ExternalEvents_SyncRuns_SyncRunId; Type: FK CONSTRAINT; Schema: Synchronization; Owner: postgres
--

ALTER TABLE ONLY "Synchronization"."ExternalEvents"
    ADD CONSTRAINT "FK_ExternalEvents_SyncRuns_SyncRunId" FOREIGN KEY ("SyncRunId") REFERENCES "Synchronization"."SyncRuns"("Id");


--
-- Name: SyncErrors FK_SyncErrors_ExternalEvents_ExternalEventId; Type: FK CONSTRAINT; Schema: Synchronization; Owner: postgres
--

ALTER TABLE ONLY "Synchronization"."SyncErrors"
    ADD CONSTRAINT "FK_SyncErrors_ExternalEvents_ExternalEventId" FOREIGN KEY ("ExternalEventId") REFERENCES "Synchronization"."ExternalEvents"("Id") ON DELETE CASCADE;


--
-- Name: SyncErrors FK_SyncErrors_SyncRuns_SyncRunId; Type: FK CONSTRAINT; Schema: Synchronization; Owner: postgres
--

ALTER TABLE ONLY "Synchronization"."SyncErrors"
    ADD CONSTRAINT "FK_SyncErrors_SyncRuns_SyncRunId" FOREIGN KEY ("SyncRunId") REFERENCES "Synchronization"."SyncRuns"("Id") ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

