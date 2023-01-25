#!/bin/bash
set -e
set -u
set -x

docker build --file ./Backbone.API/Dockerfile --tag ghcr.io/nmshd/backbone:${TAG-temp} .
