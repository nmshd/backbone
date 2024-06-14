#!/bin/bash
set -e

for f in /tmp/*.tar; do
    docker load --input $f
done
