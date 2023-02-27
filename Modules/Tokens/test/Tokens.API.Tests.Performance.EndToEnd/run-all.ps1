Param(
    [parameter(Mandatory)] $baseUrl,
    [parameter(Mandatory)] $user,
    [parameter(Mandatory)] $password,
    [parameter(Mandatory)] $size
)

Write-Host "Running tokens performance tests..."

npm install
npx webpack

Get-ChildItem -Path "./dist" -Filter *.test.js |

Foreach-Object {
    k6 run -e HOST=$baseUrl -e USER=$user -e PASSWORD=$password -e CLIENT_SECRET=test -e SIZE=$size $_.FullName
}

Write-Host "OK"