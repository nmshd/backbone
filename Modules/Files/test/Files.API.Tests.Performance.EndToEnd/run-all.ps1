Param(
    [parameter(Mandatory)] $baseUrl
)

Write-Host "Running files performance tests..."

npm install
node ".\node_modules\webpack\bin\webpack.js"

Get-ChildItem -Path "./dist" -Filter *.test.js |

Foreach-Object {
    #Large test configuration
    $VUS = 1
    $ITERATIONS = 100

    #Medium test configuration
    if(("").contains($_.Name)){
        $VUS = 1
        $ITERATIONS = 50
    }

    #Small test configuration
    if((
        "get-download.test.js",
        "post-with-auth.test.js",
        "post-without-auth.test.js"
    ).contains($_.Name)){
        $VUS = 1
        $ITERATIONS = 10
    }

    k6 run -e HOST=$baseUrl -e VUS=$VUS -e ITERATIONS=$ITERATIONS -e CLIENT_SECRET=test $_.FullName
}

Write-Host "OK"