#!/bin/bash
set -e

#for f in /tmp/*.tar; do
#    docker load --input $f
#done

for f in /tmp/*.txt; do
    docker pull $(cat $f)
done
