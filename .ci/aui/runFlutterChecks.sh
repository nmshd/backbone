#!/bin/bash
set -e
set -x

cd Applications/AdminUi
dart pub global activate melos
melos bootstrap
melos analyze
melos format
