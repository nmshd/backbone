
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

CREATE SCHEMA IF NOT EXISTS "Challenges";
CREATE SCHEMA IF NOT EXISTS "Devices";
CREATE SCHEMA IF NOT EXISTS "Files";
CREATE SCHEMA IF NOT EXISTS "Messages";
CREATE SCHEMA IF NOT EXISTS "Relationships";
CREATE SCHEMA IF NOT EXISTS "Synchronization";
CREATE SCHEMA IF NOT EXISTS "Tokens";
CREATE SCHEMA IF NOT EXISTS "Quotas";

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

ALTER USER challenges SET search_path TO "Challenges";
ALTER USER devices SET search_path TO "Devices";
ALTER USER files SET search_path TO "Files";
ALTER USER messages SET search_path TO "Messages";
ALTER USER relationships SET search_path TO "Relationships";
ALTER USER synchronization SET search_path TO "Synchronization";
ALTER USER tokens SET search_path TO "Tokens";
ALTER USER quotas SET search_path TO "Quotas";

/*+++++++++++++++++++++++++++++++++++++++++++++++++ Authorizations +++++++++++++++++++++++++++++++++++++++++++++++++*/

/*GRANT CREATE ON SCHEMA Challenges, Devices, Messages, Synchronization, Tokens, Relationships, Files TO challenges, devices, messages, synchronization, tokens, relationships, files, Quotas;
GRANT CREATE ON SCHEMA Relationships TO relationships;*/

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

GRANT USAGE ON SCHEMA "Challenges" TO devices;
GRANT SELECT ON ALL TABLES IN SCHEMA "Challenges" TO devices;

GRANT USAGE ON SCHEMA "Messages" TO quotas;
GRANT SELECT ON ALL TABLES IN SCHEMA "Messages" TO quotas;

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

/*+++++++++++++++++++++++++++++++++++++++++++++++++ Schema Owners ++++++++++++++++++++++++++++++++++++++++++++++++++*/

GRANT challenges TO "nmshdAdmin";;
GRANT devices TO "nmshdAdmin";
GRANT messages TO "nmshdAdmin";
GRANT synchronization TO "nmshdAdmin";
GRANT tokens TO "nmshdAdmin";
GRANT relationships TO "nmshdAdmin";
GRANT files TO "nmshdAdmin";
GRANT quotas TO "nmshdAdmin";

ALTER SCHEMA "Challenges" OWNER TO challenges;
ALTER SCHEMA "Devices" OWNER TO devices;
ALTER SCHEMA "Messages" OWNER TO messages;
ALTER SCHEMA "Synchronization" OWNER TO synchronization;
ALTER SCHEMA "Tokens" OWNER TO tokens;
ALTER SCHEMA "Relationships" OWNER TO relationships;
ALTER SCHEMA "Files" OWNER TO files;
ALTER SCHEMA "Quotas" OWNER TO quotas;
