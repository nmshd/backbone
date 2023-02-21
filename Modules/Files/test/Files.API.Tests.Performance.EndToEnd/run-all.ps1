Param(
    [parameter(Mandatory)] $baseUrl,
    [parameter(Mandatory)] $size
)

Write-Host "Running files performance tests..."

npm install
node ".\node_modules\webpack\bin\webpack.js"

Get-ChildItem -Path "./dist" -Filter *.test.js |

Foreach-Object {
    k6 run -e HOST=$baseUrl -e SIZE=$size -e CLIENT_SECRET=test $_.FullName
}

Write-Host "OK"