Param(
    [parameter(Mandatory)] $baseurl,
    [parameter(Mandatory)] $username,
    [parameter(Mandatory)] $password,
    [parameter(Mandatory)] $size
)

Write-Host "Running files performance tests..."

npm install
npx webpack

Get-ChildItem -Path "./dist" -Filter *.test.js |

Foreach-Object {
    k6 run -e HOST=$baseurl -e USERNAME=$username -e PASSWORD=$password -e CLIENT_SECRET=test -e SIZE=$size $_.FullName
}

Write-Host "OK"