#!/bin/bash
set -e
dotnet restore $(pwd)/src/Rng/
rm -rf $(pwd)/deployment/build/rng
dotnet publish $(pwd)/src/Rng/ -r ubuntu.16.04-x64 -o $(pwd)/deployment/build/rng -c Release