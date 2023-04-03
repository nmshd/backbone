#!/usr/bin/env -S npx ts-node --esm
import { $ } from "zx";

await $`dotnet restore "Backbone.sln"`;
await $`dotnet build  /property:WarningLevel=0 --no-restore "Backbone.sln"`;
await $`dotnet test --no-restore --no-build "Backbone.sln" --filter "Category!=Integration"`;
