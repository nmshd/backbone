#!/bin/bash
set -e
set -x

INITIAL_DIR=$(pwd)
cd Applications/AdminUi
dart pub global activate melos
melos bootstrap
melos analyze
melos format
