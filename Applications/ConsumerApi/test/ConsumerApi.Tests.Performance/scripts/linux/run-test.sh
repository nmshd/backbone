#!/bin/bash

# Get the scenario, either from the command line argument or by prompting the user
s="$1"
if [ -z "$s" ]; then
  read -p "Enter the string: " s
fi

# Capture all arguments after `--`
while [ "$1" != "--" ] && [ -n "$1" ]; do
    s="$1"
    shift
done

# Shift past `--`
if [ "$1" == "--" ]; then
    shift
fi

# Remaining arguments are for `k6`
k6Arguments="$@"

# Generate a timestamp `t` in the format YYYYMMDD-HHmmSS
t=$(date +"%Y%m%d-%H%M%S")

# Construct the file paths and commands
testFile="./dist/${s}.test.js"
outputFile="./k6-outputs/${t}-${s}.csv"
resultAnalyzerScript="./tools/result-analyzer/src/main.js"

# ensure the the result analyzer has its dependencies installed

cd tools/result-analyzer

npm i

cd ../..

mkdir -p k6-results

# Run the `npx webpack` command
npx webpack

# Check the exit status of the webpack command
if [ $? -ne 0 ]; then
  echo "Error: Webpack failed."
  exit 1
fi

# Check if the test file exists
if [ ! -f "$testFile" ]; then
  echo "Error: Test file '$testFile' does not exist."
  exit 1
fi

# Run the `k6` command
k6 run "$testFile" -o "csv=$outputFile" $k6Arguments

# Run the result analyzer script
node "$resultAnalyzerScript" "$outputFile"
