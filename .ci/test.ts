#!/usr/bin/env -S npx ts-node --esm
import { $ } from "zx";

$`dotnet restore "Backbone.sln"`;
$`dotnet build  /property:WarningLevel=0 --no-restore "Backbone.sln"`;
$`dotnet test --no-restore --no-build "Backbone.sln" --filter "Category!=Integration"`;
