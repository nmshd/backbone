#!/bin/bash
set -e
set -x

cd AdminUi
dart pub global activate melos
melos bootstrap
melos analyze
melos format
