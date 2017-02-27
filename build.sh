#!/usr/bin/env bash
dotnet restore src/Rng/
dotnet publish src/Rng/ -r ubuntu.14.04-x64 -o ./artifacts/rng -c Release