/*++++++++++++++++++++++++++++++++++++++++++++++++++++ Create Schemas +++++++++++++++++++++++++++++++++++++++++++++++++++++*/

CREATE SCHEMA IF NOT EXISTS "Challenges";
CREATE SCHEMA IF NOT EXISTS "Devices";
CREATE SCHEMA IF NOT EXISTS "Files";
CREATE SCHEMA IF NOT EXISTS "Messages";
CREATE SCHEMA IF NOT EXISTS "Relationships";
CREATE SCHEMA IF NOT EXISTS "Synchronization";
CREATE SCHEMA IF NOT EXISTS "Tokens";
CREATE SCHEMA IF NOT EXISTS "Quotas";
CREATE SCHEMA IF NOT EXISTS "AdminUi";

/*+++++++++++++++++++++++++++++++++++++++++++++++++ Grant permissions to admin user ++++++++++++++++++++++++++++++++++++++++++++++++++*/

GRANT challenges TO CURRENT_USER;
GRANT devices TO CURRENT_USER;
GRANT messages TO CURRENT_USER;
GRANT synchronization TO CURRENT_USER;
GRANT tokens TO CURRENT_USER;
GRANT relationships TO CURRENT_USER;
GRANT files TO CURRENT_USER;
GRANT quotas TO CURRENT_USER;
GRANT "adminUi" TO CURRENT_USER;

/*+++++++++++++++++++++++++++++++++++++++++++++++++++++ Set default schemas ++++++++++++++++++++++++++++++++++++++++++++++++++++++*/

ALTER USER challenges SET search_path TO "Challenges";
ALTER USER devices SET search_path TO "Devices";
ALTER USER files SET search_path TO "Files";
ALTER USER messages SET search_path TO "Messages";
ALTER USER relationships SET search_path TO "Relationships";
ALTER USER synchronization SET search_path TO "Synchronization";
ALTER USER tokens SET search_path TO "Tokens";
ALTER USER quotas SET search_path TO "Quotas";
ALTER USER "adminUi" SET search_path TO "AdminUi";

/*+++++++++++++++++++++++++++++++++++++++++++++++++ Grant permissions +++++++++++++++++++++++++++++++++++++++++++++++++*/

REVOKE USAGE, CREATE ON SCHEMA "Challenges" FROM synchronization, devices, messages, tokens, relationships, files, quotas;
REVOKE USAGE, CREATE ON SCHEMA "Synchronization" FROM challenges, devices, messages, tokens, relationships, files, quotas;
REVOKE USAGE, CREATE ON SCHEMA "Messages" FROM challenges, synchronization, devices, tokens, relationships, files, quotas;
REVOKE USAGE, CREATE ON SCHEMA "Devices" FROM challenges, synchronization, messages, tokens, relationships, files, quotas;
REVOKE USAGE, CREATE ON SCHEMA "Tokens" FROM challenges, synchronization, devices, messages, relationships, files, quotas;
REVOKE USAGE, CREATE ON SCHEMA "Relationships" FROM challenges, synchronization, devices, messages, tokens, files, quotas;
REVOKE USAGE, CREATE ON SCHEMA "Files" FROM challenges, synchronization, devices, messages, tokens, relationships, quotas;
REVOKE USAGE, CREATE ON SCHEMA "Quotas" FROM challenges, synchronization, devices, messages, tokens, relationships, files;

GRANT USAGE ON SCHEMA "Relationships" TO messages;
GRANT SELECT, REFERENCES, TRIGGER, TRUNCATE ON ALL TABLES IN SCHEMA "Relationships" TO messages;
ALTER DEFAULT PRIVILEGES FOR ROLE relationships IN SCHEMA "Relationships" GRANT SELECT, REFERENCES, TRIGGER, TRUNCATE ON TABLES TO messages;

GRANT USAGE ON SCHEMA "Challenges" TO devices;
GRANT SELECT ON ALL TABLES IN SCHEMA "Challenges" TO devices;
ALTER DEFAULT PRIVILEGES FOR ROLE challenges IN SCHEMA "Challenges" GRANT SELECT ON TABLES TO devices;

GRANT USAGE ON SCHEMA "Messages" TO quotas;
GRANT SELECT ON ALL TABLES IN SCHEMA "Messages" TO quotas;
ALTER DEFAULT PRIVILEGES FOR ROLE messages IN SCHEMA "Messages" GRANT SELECT ON TABLES TO quotas;

GRANT USAGE ON SCHEMA "Files" TO quotas;
GRANT SELECT ON ALL TABLES IN SCHEMA "Files" TO quotas;
ALTER DEFAULT PRIVILEGES FOR ROLE files IN SCHEMA "Files" GRANT SELECT ON TABLES TO quotas;

GRANT USAGE ON SCHEMA "Relationships" TO quotas;
GRANT SELECT ON ALL TABLES IN SCHEMA "Relationships" TO quotas;
ALTER DEFAULT PRIVILEGES FOR ROLE relationships IN SCHEMA "Relationships" GRANT SELECT ON TABLES TO quotas;

GRANT USAGE ON SCHEMA "Challenges" TO "adminUi";
GRANT SELECT ON ALL TABLES IN SCHEMA "Challenges" TO "adminUi";
ALTER DEFAULT PRIVILEGES FOR ROLE challenges IN SCHEMA "Challenges" GRANT SELECT ON TABLES TO "adminUi";

GRANT USAGE ON SCHEMA "Synchronization" TO "adminUi";
GRANT SELECT ON ALL TABLES IN SCHEMA "Synchronization" TO "adminUi";
ALTER DEFAULT PRIVILEGES FOR ROLE synchronization IN SCHEMA "Synchronization" GRANT SELECT ON TABLES TO "adminUi";

GRANT USAGE ON SCHEMA "Messages" TO "adminUi";
GRANT SELECT ON ALL TABLES IN SCHEMA "Messages" TO "adminUi";
ALTER DEFAULT PRIVILEGES FOR ROLE messages IN SCHEMA "Messages" GRANT SELECT ON TABLES TO "adminUi";

GRANT USAGE ON SCHEMA "Devices" TO "adminUi";
GRANT SELECT ON ALL TABLES IN SCHEMA "Devices" TO "adminUi";
ALTER DEFAULT PRIVILEGES FOR ROLE devices IN SCHEMA "Devices" GRANT SELECT ON TABLES TO "adminUi";

GRANT USAGE ON SCHEMA "Tokens" TO "adminUi";
GRANT SELECT ON ALL TABLES IN SCHEMA "Tokens" TO "adminUi";
ALTER DEFAULT PRIVILEGES FOR ROLE tokens IN SCHEMA "Tokens" GRANT SELECT ON TABLES TO "adminUi";

GRANT USAGE ON SCHEMA "Relationships" TO "adminUi";
GRANT SELECT ON ALL TABLES IN SCHEMA "Relationships" TO "adminUi";
ALTER DEFAULT PRIVILEGES FOR ROLE relationships IN SCHEMA "Relationships" GRANT SELECT ON TABLES TO "adminUi";

GRANT USAGE ON SCHEMA "Files" TO "adminUi";
GRANT SELECT ON ALL TABLES IN SCHEMA "Files" TO "adminUi";
ALTER DEFAULT PRIVILEGES FOR ROLE files IN SCHEMA "Files" GRANT SELECT ON TABLES TO "adminUi";

GRANT USAGE ON SCHEMA "Quotas" TO "adminUi";
GRANT SELECT ON ALL TABLES IN SCHEMA "Quotas" TO "adminUi";
ALTER DEFAULT PRIVILEGES FOR ROLE quotas IN SCHEMA "Quotas" GRANT SELECT ON TABLES TO "adminUi";

GRANT USAGE ON SCHEMA "Tokens" TO quotas;
GRANT SELECT ON ALL TABLES IN SCHEMA "Tokens" TO quotas;
ALTER DEFAULT PRIVILEGES FOR ROLE tokens IN SCHEMA "Tokens" GRANT SELECT ON TABLES TO quotas;

/*+++++++++++++++++++++++++++++++++++++++++++++++++ Create __EFMigrationsHistory tables +++++++++++++++++++++++++++++++++++++++++++++++++*/

CREATE TABLE IF NOT EXISTS "Challenges"."__EFMigrationsHistory"
(
    "MigrationId" character varying(150) COLLATE pg_catalog."default" NOT NULL,
    "ProductVersion" character varying(32) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);
ALTER TABLE IF EXISTS "Challenges"."__EFMigrationsHistory" OWNER to challenges;

CREATE TABLE IF NOT EXISTS "Devices"."__EFMigrationsHistory"
(
    "MigrationId" character varying(150) COLLATE pg_catalog."default" NOT NULL,
    "ProductVersion" character varying(32) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);
ALTER TABLE IF EXISTS "Devices"."__EFMigrationsHistory" OWNER to devices;

CREATE TABLE IF NOT EXISTS "Files"."__EFMigrationsHistory"
(
    "MigrationId" character varying(150) COLLATE pg_catalog."default" NOT NULL,
    "ProductVersion" character varying(32) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);
ALTER TABLE IF EXISTS "Files"."__EFMigrationsHistory" OWNER to files;

CREATE TABLE IF NOT EXISTS "Messages"."__EFMigrationsHistory"
(
    "MigrationId" character varying(150) COLLATE pg_catalog."default" NOT NULL,
    "ProductVersion" character varying(32) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);
ALTER TABLE IF EXISTS "Messages"."__EFMigrationsHistory" OWNER to messages;

CREATE TABLE IF NOT EXISTS "Relationships"."__EFMigrationsHistory"
(
    "MigrationId" character varying(150) COLLATE pg_catalog."default" NOT NULL,
    "ProductVersion" character varying(32) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);
ALTER TABLE IF EXISTS "Relationships"."__EFMigrationsHistory" OWNER to relationships;

CREATE TABLE IF NOT EXISTS "Synchronization"."__EFMigrationsHistory"
(
    "MigrationId" character varying(150) COLLATE pg_catalog."default" NOT NULL,
    "ProductVersion" character varying(32) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);
ALTER TABLE IF EXISTS "Synchronization"."__EFMigrationsHistory" OWNER to synchronization;

CREATE TABLE IF NOT EXISTS "Tokens"."__EFMigrationsHistory"
(
    "MigrationId" character varying(150) COLLATE pg_catalog."default" NOT NULL,
    "ProductVersion" character varying(32) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);
ALTER TABLE IF EXISTS "Tokens"."__EFMigrationsHistory" OWNER to tokens;

CREATE TABLE IF NOT EXISTS "Quotas"."__EFMigrationsHistory"
(
    "MigrationId" character varying(150) COLLATE pg_catalog."default" NOT NULL,
    "ProductVersion" character varying(32) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);
ALTER TABLE IF EXISTS "Quotas"."__EFMigrationsHistory" OWNER to quotas;

CREATE TABLE IF NOT EXISTS "AdminUi"."__EFMigrationsHistory"
(
    "MigrationId" character varying(150) COLLATE pg_catalog."default" NOT NULL,
    "ProductVersion" character varying(32) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);
ALTER TABLE IF EXISTS "AdminUi"."__EFMigrationsHistory" OWNER to "adminUi";

/*+++++++++++++++++++++++++++++++++++++++++++++++++ Set Schema Owners ++++++++++++++++++++++++++++++++++++++++++++++++++*/

ALTER SCHEMA "Challenges" OWNER TO challenges;
ALTER SCHEMA "Devices" OWNER TO devices;
ALTER SCHEMA "Messages" OWNER TO messages;
ALTER SCHEMA "Synchronization" OWNER TO synchronization;
ALTER SCHEMA "Tokens" OWNER TO tokens;
ALTER SCHEMA "Relationships" OWNER TO relationships;
ALTER SCHEMA "Files" OWNER TO files;
ALTER SCHEMA "Quotas" OWNER TO quotas;
ALTER SCHEMA "AdminUi" OWNER TO "adminUi";
