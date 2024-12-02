
/********************************************** Database Configuration **********************************************/

/*++++++++++++++++++++++++++++++++++++++++++++++++++++ Schemas +++++++++++++++++++++++++++++++++++++++++++++++++++++*/

/*
DROP SCHEMA "Challenges" cascade;
DROP SCHEMA "Devices" cascade;
DROP SCHEMA "Files" cascade;
DROP SCHEMA "Messages" cascade;
DROP SCHEMA "Relationships" cascade;
DROP SCHEMA "Synchronization" cascade;
DROP SCHEMA "Tokens" cascade;
DROP SCHEMA "Quotas" cascade;
*/

CREATE SCHEMA IF NOT EXISTS "Announcements";
CREATE SCHEMA IF NOT EXISTS "Challenges";
CREATE SCHEMA IF NOT EXISTS "Devices";
CREATE SCHEMA IF NOT EXISTS "Files";
CREATE SCHEMA IF NOT EXISTS "Messages";
CREATE SCHEMA IF NOT EXISTS "Relationships";
CREATE SCHEMA IF NOT EXISTS "Synchronization";
CREATE SCHEMA IF NOT EXISTS "Tokens";
CREATE SCHEMA IF NOT EXISTS "Quotas";
CREATE SCHEMA IF NOT EXISTS "AdminUi";

/*+++++++++++++++++++++++++++++++++++++++++++++++++++++ Users ++++++++++++++++++++++++++++++++++++++++++++++++++++++*/

/* DROP USER challenges, devices, messages, files, relationships, synchronization, tokens, quotas */ 
 
DO
$$
BEGIN
   IF NOT EXISTS (SELECT usename FROM pg_user WHERE usename = 'nmshdAdmin') THEN
      CREATE USER "nmshdAdmin" WITH password 'Passw0rd';
      RAISE NOTICE 'User "nmshdAdmin" created';
   END IF;
END
$$;

DO
$$
BEGIN
   IF NOT EXISTS (SELECT usename FROM pg_user WHERE usename = 'announcements') THEN
      CREATE USER "announcements" WITH password 'Passw0rd';
      RAISE NOTICE 'User "announcements" created';
   END IF;
END
$$;

DO
$$
BEGIN
   IF NOT EXISTS (SELECT usename FROM pg_user WHERE usename = 'challenges') THEN
      CREATE USER challenges WITH password 'Passw0rd';
      RAISE NOTICE 'User "challenges" created';
   END IF;
END
$$;

DO
$$
BEGIN
   IF NOT EXISTS (SELECT usename FROM pg_user WHERE usename = 'devices') THEN
      CREATE USER devices WITH password 'Passw0rd';
      RAISE NOTICE 'User "devices" created';
   END IF;
END
$$;

DO
$$
BEGIN
   IF NOT EXISTS (SELECT usename FROM pg_user WHERE usename = 'messages') THEN
      CREATE USER messages WITH password 'Passw0rd';
      RAISE NOTICE 'User "messages" created';
   END IF;
END
$$;

DO
$$
BEGIN
   IF NOT EXISTS (SELECT usename FROM pg_user WHERE usename = 'synchronization') THEN
      CREATE USER synchronization WITH password 'Passw0rd';
      RAISE NOTICE 'User "synchronization" created';
   END IF;
END
$$;

DO
$$
BEGIN
   IF NOT EXISTS (SELECT usename FROM pg_user WHERE usename = 'tokens') THEN
      CREATE USER tokens WITH password 'Passw0rd';
      RAISE NOTICE 'User "tokens" created';
   END IF;
END
$$;

DO
$$
BEGIN
   IF NOT EXISTS (SELECT usename FROM pg_user WHERE usename = 'files') THEN
      CREATE USER files WITH password 'Passw0rd';
      RAISE NOTICE 'User "files" created';
   END IF;
END
$$;

DO
$$
BEGIN
   IF NOT EXISTS (SELECT usename FROM pg_user WHERE usename = 'relationships') THEN
      CREATE USER relationships WITH password 'Passw0rd';
      RAISE NOTICE 'User "relationships" created';
   END IF;
END
$$;

DO
$$
BEGIN
   IF NOT EXISTS (SELECT usename FROM pg_user WHERE usename = 'quotas') THEN
      CREATE USER quotas WITH password 'Passw0rd';
      RAISE NOTICE 'User "quotas" created';
   END IF;
END
$$;

DO
$$
BEGIN
   IF NOT EXISTS (SELECT usename FROM pg_user WHERE usename = 'adminUi') THEN
      CREATE USER "adminUi" WITH password 'Passw0rd';
      RAISE NOTICE 'User "adminUi" created';
   END IF;
END
$$;

/*+++++++++++++++++++++++++++++++++++++++++++++++++ Authorizations +++++++++++++++++++++++++++++++++++++++++++++++++*/

GRANT USAGE ON SCHEMA "Announcements" TO announcements;
GRANT USAGE ON SCHEMA "Challenges" TO challenges;
GRANT USAGE ON SCHEMA "Devices" TO devices;
GRANT USAGE ON SCHEMA "Files" TO files;
GRANT USAGE ON SCHEMA "Messages" TO messages;
GRANT USAGE ON SCHEMA "Quotas" TO quotas;
GRANT USAGE ON SCHEMA "Relationships" TO relationships;
GRANT USAGE ON SCHEMA "Synchronization" TO synchronization;
GRANT USAGE ON SCHEMA "Tokens" TO tokens;
GRANT USAGE ON SCHEMA "AdminUi" TO "adminUi";

ALTER DEFAULT PRIVILEGES IN SCHEMA "Announcements" GRANT ALL ON TABLES TO announcements;
ALTER DEFAULT PRIVILEGES IN SCHEMA "Challenges" GRANT ALL ON TABLES TO challenges;
ALTER DEFAULT PRIVILEGES IN SCHEMA "Devices" GRANT ALL ON TABLES TO devices;
ALTER DEFAULT PRIVILEGES IN SCHEMA "Files" GRANT ALL ON TABLES TO files;
ALTER DEFAULT PRIVILEGES IN SCHEMA "Messages" GRANT ALL ON TABLES TO messages;
ALTER DEFAULT PRIVILEGES IN SCHEMA "Quotas" GRANT ALL ON TABLES TO quotas;
ALTER DEFAULT PRIVILEGES IN SCHEMA "Relationships" GRANT ALL ON TABLES TO relationships;
ALTER DEFAULT PRIVILEGES IN SCHEMA "Synchronization" GRANT ALL ON TABLES TO synchronization;
ALTER DEFAULT PRIVILEGES IN SCHEMA "Tokens" GRANT ALL ON TABLES TO tokens;
ALTER DEFAULT PRIVILEGES IN SCHEMA "AdminUi" GRANT ALL ON TABLES TO "adminUi";

GRANT USAGE ON SCHEMA "Relationships" TO messages;
GRANT SELECT, REFERENCES, TRIGGER, TRUNCATE ON ALL TABLES IN SCHEMA "Relationships" TO messages;
ALTER DEFAULT PRIVILEGES IN SCHEMA "Relationships" GRANT SELECT, REFERENCES, TRIGGER, TRUNCATE ON TABLES TO messages;

GRANT USAGE ON SCHEMA "Challenges" TO devices;
GRANT SELECT ON ALL TABLES IN SCHEMA "Challenges" TO devices;
ALTER DEFAULT PRIVILEGES IN SCHEMA "Challenges" GRANT SELECT ON TABLES TO devices;

GRANT USAGE ON SCHEMA "Challenges" TO quotas;
GRANT SELECT ON ALL TABLES IN SCHEMA "Challenges" TO quotas;
ALTER DEFAULT PRIVILEGES IN SCHEMA "Challenges" GRANT SELECT ON TABLES TO quotas;

GRANT USAGE ON SCHEMA "Devices" TO quotas;
GRANT SELECT ON ALL TABLES IN SCHEMA "Devices" TO quotas;
ALTER DEFAULT PRIVILEGES IN SCHEMA "Devices" GRANT SELECT ON TABLES TO quotas;

GRANT USAGE ON SCHEMA "Files" TO quotas;
GRANT SELECT ON ALL TABLES IN SCHEMA "Files" TO quotas;
ALTER DEFAULT PRIVILEGES IN SCHEMA "Files" GRANT SELECT ON TABLES TO quotas;

GRANT USAGE ON SCHEMA "Messages" TO quotas;
GRANT SELECT ON ALL TABLES IN SCHEMA "Messages" TO quotas;
ALTER DEFAULT PRIVILEGES IN SCHEMA "Messages" GRANT SELECT ON TABLES TO quotas;

GRANT USAGE ON SCHEMA "Relationships" TO synchronization;
GRANT SELECT ON ALL TABLES IN SCHEMA "Relationships" TO synchronization;
ALTER DEFAULT PRIVILEGES IN SCHEMA "Relationships" GRANT SELECT ON TABLES TO synchronization;

GRANT USAGE ON SCHEMA "Relationships" TO quotas;
GRANT SELECT ON ALL TABLES IN SCHEMA "Relationships" TO quotas;
ALTER DEFAULT PRIVILEGES IN SCHEMA "Relationships" GRANT SELECT ON TABLES TO quotas;

GRANT USAGE ON SCHEMA "Synchronization" TO quotas;
GRANT SELECT ON ALL TABLES IN SCHEMA "Synchronization" TO quotas;
ALTER DEFAULT PRIVILEGES IN SCHEMA "Synchronization" GRANT SELECT ON TABLES TO quotas;

GRANT USAGE ON SCHEMA "Tokens" TO quotas;
GRANT SELECT ON ALL TABLES IN SCHEMA "Tokens" TO quotas;
ALTER DEFAULT PRIVILEGES IN SCHEMA "Tokens" GRANT SELECT ON TABLES TO quotas;

GRANT USAGE ON SCHEMA "Challenges" TO "adminUi";
GRANT SELECT ON ALL TABLES IN SCHEMA "Challenges" TO "adminUi";
ALTER DEFAULT PRIVILEGES IN SCHEMA "Challenges" GRANT SELECT ON TABLES TO "adminUi";

GRANT USAGE ON SCHEMA "Synchronization" TO "adminUi";
GRANT SELECT ON ALL TABLES IN SCHEMA "Synchronization" TO "adminUi";
ALTER DEFAULT PRIVILEGES IN SCHEMA "Synchronization" GRANT SELECT ON TABLES TO "adminUi";

GRANT USAGE ON SCHEMA "Messages" TO "adminUi";
GRANT SELECT ON ALL TABLES IN SCHEMA "Messages" TO "adminUi";
ALTER DEFAULT PRIVILEGES IN SCHEMA "Messages" GRANT SELECT ON TABLES TO "adminUi";

GRANT USAGE ON SCHEMA "Devices" TO "adminUi";
GRANT SELECT ON ALL TABLES IN SCHEMA "Devices" TO "adminUi";
ALTER DEFAULT PRIVILEGES IN SCHEMA "Devices" GRANT SELECT ON TABLES TO "adminUi";

GRANT USAGE ON SCHEMA "Tokens" TO "adminUi";
GRANT SELECT ON ALL TABLES IN SCHEMA "Tokens" TO "adminUi";
ALTER DEFAULT PRIVILEGES IN SCHEMA "Tokens" GRANT SELECT ON TABLES TO "adminUi";

GRANT USAGE ON SCHEMA "Relationships" TO "adminUi";
GRANT SELECT ON ALL TABLES IN SCHEMA "Relationships" TO "adminUi";
ALTER DEFAULT PRIVILEGES IN SCHEMA "Relationships" GRANT SELECT ON TABLES TO "adminUi";

GRANT USAGE ON SCHEMA "Files" TO "adminUi";
GRANT SELECT ON ALL TABLES IN SCHEMA "Files" TO "adminUi";
ALTER DEFAULT PRIVILEGES IN SCHEMA "Files" GRANT SELECT ON TABLES TO "adminUi";

GRANT USAGE ON SCHEMA "Quotas" TO "adminUi";
GRANT SELECT ON ALL TABLES IN SCHEMA "Quotas" TO "adminUi";
ALTER DEFAULT PRIVILEGES IN SCHEMA "Quotas" GRANT SELECT ON TABLES TO "adminUi";

GRANT USAGE ON SCHEMA "Tokens" TO quotas;
GRANT SELECT ON ALL TABLES IN SCHEMA "Tokens" TO quotas;
ALTER DEFAULT PRIVILEGES IN SCHEMA "Tokens" GRANT SELECT ON TABLES TO quotas;

CREATE TABLE IF NOT EXISTS "Announcements"."__EFMigrationsHistory"
(
    "MigrationId" character varying(150) COLLATE pg_catalog."default" NOT NULL,
    "ProductVersion" character varying(32) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);
ALTER TABLE IF EXISTS "Announcements"."__EFMigrationsHistory" OWNER to announcements;

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

/*+++++++++++++++++++++++++++++++++++++++++++++++++ Schema Owners ++++++++++++++++++++++++++++++++++++++++++++++++++*/

GRANT challenges TO "announcements";
GRANT challenges TO "nmshdAdmin";
GRANT devices TO "nmshdAdmin";
GRANT messages TO "nmshdAdmin";
GRANT synchronization TO "nmshdAdmin";
GRANT tokens TO "nmshdAdmin";
GRANT relationships TO "nmshdAdmin";
GRANT files TO "nmshdAdmin";
GRANT quotas TO "nmshdAdmin";
GRANT "adminUi" TO "nmshdAdmin";
