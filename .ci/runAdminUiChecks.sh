set -e
set -x

INITIAL_DIR=$(pwd)
cd AdminUi/src/AdminUi/ClientApp
npm ci
npx eslint --ext ts ./src
npx prettier --check .
npx license-check --ignorePackages adminui@0.0.0
npx better-npm-audit audit
cd $INITIAL_DIR
