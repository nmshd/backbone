#! /bin/bash

docker tag consumer-api localhost:5000/test-consumerapi && docker push localhost:5000/consumerapi || true
docker tag admin-ui localhost:5000/test-adminui && docker push localhost:5000/adminui || true
docker tag seed-client localhost:5000/test-seed && docker push localhost:5000/seed || true
