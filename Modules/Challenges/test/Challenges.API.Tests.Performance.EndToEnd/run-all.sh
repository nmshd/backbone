
#!/bin/bash
while getopts h:u:p:s: opt
do
    case $opt in
        h) BASEURL=$OPTARG ;;
        u) USERNAME=$OPTARG ;;
        p) PASSWORD=$OPTARG ;;
        s) SIZE=$OPTARG ;;
    esac
done

echo "Running challenges performance tests..."

npm install
npx webpack

for file in "./dist/"*.test.js ; do 
    k6 run -e HOST=$BASEURL -e USERNAME=$USERNAME -e PASSWORD=$PASSWORD -e CLIENT_SECRET=test -e SIZE=$SIZE $file
done

echo "OK"