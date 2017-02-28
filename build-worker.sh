#!/bin/bash
set -e
dotnet restore $(pwd)/src/Worker/
rm -rf $(pwd)/deployment/build/worker
dotnet publish $(pwd)/src/Worker/ -o $(pwd)/deployment/build/worker -c Release