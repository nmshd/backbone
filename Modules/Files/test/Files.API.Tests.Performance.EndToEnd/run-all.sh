#!/bin/bash
while getopts h:u:p:s: opt
do
    case $opt in
        h) BASEURL=$OPTARG ;;
        u) USER=$OPTARG ;;
        p) PASSWORD=$OPTARG ;;
        s) SIZE=$OPTARG ;;
    esac
done

echo "Running files performance tests..."

npm install
npx webpack

for file in "./dist/"*.test.js ; do 
    k6 run -e HOST=$BASEURL -e USER=$USERNAME -e PASSWORD=$PASSWORD -e CLIENT_SECRET=test -e SIZE=$SIZE $file
done

echo "OK"