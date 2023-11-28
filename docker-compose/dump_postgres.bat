
docker exec --env PGPASSWORD=admin -it bkb-postgres pg_dump -h postgres -U postgres --create enmeshed -f tmp/enmeshed.pg
docker cp bkb-postgres:/tmp/enmeshed.pg ./dumps/enmeshed.pg