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

DO
$$
BEGIN
   IF NOT EXISTS (SELECT usename FROM pg_user WHERE usename = 'adminUi') THEN
      CREATE USER "adminUi" WITH password 'Passw0rd';
      RAISE NOTICE 'User "adminUi" created';
   END IF;
END
$$;
