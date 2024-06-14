#!/bin/bash

for f in /tmp/*.tar; do
    docker load --input $f
done
