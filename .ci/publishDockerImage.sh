#!/bin/bash
set -e
set -u
set -x

docker push ghcr.io/nmshd/backbone:${TAG}
