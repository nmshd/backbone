Param(
    [parameter(Mandatory)] $baseUrl,
    [parameter(Mandatory)] $username,
    [parameter(Mandatory)] $password,
    [parameter(Mandatory)] $size
)

Write-Host "Running Relationship Templates performance tests..."

npm install
npx webpack

Get-ChildItem -Path "./dist" -Filter *.test.js |

Foreach-Object {
    k6 run -e HOST=$baseUrl -e USERNAME=$username -e PASSWORD=$password -e CLIENT_SECRET=test -e SIZE=$size $_.FullName
}

Write-Host "OK"