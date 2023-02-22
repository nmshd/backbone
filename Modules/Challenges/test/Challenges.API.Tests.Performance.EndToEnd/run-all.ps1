Param(
    [parameter(Mandatory)] $baseUrl,
    [parameter(Mandatory)] $username,
    [parameter(Mandatory)] $password,
    [parameter(Mandatory)] $size
)

Write-Host "Running challenges performance tests..."

npm install
node "./node_modules/webpack/bin/webpack.js"

Get-ChildItem -Path "./dist" -Filter *.test.js |

Foreach-Object {
    k6 run -e HOST=$baseUrl -e USERNAME=$username -e PASSWORD=$password -e CLIENT_SECRET=test -e SIZE=$size $_.FullName
}

Write-Host "OK"