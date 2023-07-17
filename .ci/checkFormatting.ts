#!/usr/bin/env -S npx ts-node --esm -T

import { $ } from "zx";

$`dotnet format --verify-no-changes`;
