#!/usr/bin/python3

import sys
import subprocess

POSTGRES_PARTS = [
    "docker",
    "compose",
    "-f",
    "./.ci/docker-compose.test.yml",
    "-f",
    "./.ci/docker-compose.test.postgres.yml"
]

SQLSERVER_PARTS = [
    "docker",
    "compose",
    "-f",
    "./.ci/docker-compose.test.yml",
    "-f",
    "./.ci/docker-compose.test.sqlserver.yml"
]

RESTORE_PARTS = [
    "dotnet",
    "restore",
    "Backbone.sln"
]

BUILD_PARTS = [
    "dotnet",
    "build",
    "--no-restore",
    "Backbone.sln"
]

TEST_PARTS = [
    "dotnet",
    "test",
    "--no-restore",
    "--no-build",
    "--filter",
    "Category=Integration&TestCategory!~ignore",
    "--logger",
    "GitHubActions;summary.includePassedTests=true;summary.includeSkippedTests=true",
    "Backbone.sln"
]

def runCmd(args: list[str]) -> int:
    return subprocess.run(args).returncode

def docker_compose_postgres(args: list[str]) -> int:
    parts = POSTGRES_PARTS.copy()

    for a in args:
        parts.append(a)
    
    return runCmd(parts)

def docker_compose_sqlserver(args: list[str]) -> int:
    parts = SQLSERVER_PARTS.copy()

    for a in args:
        parts.append(a)
    
    return runCmd(parts)

def build_and_run_tests() -> int:
    ret = runCmd(RESTORE_PARTS)

    if ret != 0:
        ret = runCmd(BUILD_PARTS)
        if ret != 0:
            ret = runCmd(TEST_PARTS)

    return ret


if __name__ == "__main__":
    ret = 1

    if len(sys.argv) < 2:
        print("No arguments", file=sys.stderr)
    elif sys.argv[1] == "postgres":
        ret = docker_compose_postgres(sys.argv[2:])
    elif sys.argv[1] == "sqlserver":
        ret = docker_compose_sqlserver(sys.argv[2:])
    elif sys.argv[1] == "build":
        ret = build_and_run_tests()
    
    exit(ret)
