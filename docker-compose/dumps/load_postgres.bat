docker cp ./dumps/enmeshed.pg bkb-postgres:/tmp/enmeshed.sql

docker exec --env PGPASSWORD=admin -it bkb-postgres psql -h postgres -U postgres postgres -c "DELETE DATABASE enmeshed;"
docker exec --env PGPASSWORD=admin -it bkb-postgres psql -h postgres -U postgres postgres -c "CREATE DATABASE enmeshed WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'en_US.utf8'"
docker exec --env PGPASSWORD=admin -it bkb-postgres psql -h postgres -U postgres postgres -c "ALTER DATABASE enmeshed OWNER TO postgres;"
docker exec --env PGPASSWORD=admin -it bkb-postgres psql -h postgres -U postgres enmeshed -f /tmp/enmeshed.sql