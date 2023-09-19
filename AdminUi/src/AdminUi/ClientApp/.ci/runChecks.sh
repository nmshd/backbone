set -e
set -x

npm ci
npx eslint --ext ts ./src
npx prettier --check .
npx license-check --ignorePackages adminui@0.0.0

npx better-npm-audit audit
