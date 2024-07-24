#!/bin/bash
set -e
set -x

INITIAL_DIR=$(pwd)
cd Applications/AdminApi/src/AdminApi/ClientApp
npm ci
npx eslint --ext ts ./src
npx prettier --check .
npx license-check --ignorePackages adminui@0.0.0
npx better-npm-audit audit --exclude 1098115
cd $INITIAL_DIR
