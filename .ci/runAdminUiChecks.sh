set -e
set -x

INITIAL_DIR=$(pwd)
cd AdminUi/src/AdminUi/ClientApp
npm ci
npx eslint --ext ts ./src
npx prettier --check .
npx license-check --ignorePackages adminui@0.0.0 path-scurry@1.10.1 jackspeak@2.3.6
npx better-npm-audit audit --exclude 1094889
cd $INITIAL_DIR
