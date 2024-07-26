#!/bin/bash
set -e
set -x

INITIAL_DIR=$(pwd)
cd Applications/AdminApi/src/AdminApi/ClientApp
dart pub global activate melos
melos bootstrap
melos analyze
melos format
