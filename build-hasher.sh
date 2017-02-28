#!/bin/bash
set -e
dotnet restore $(pwd)/src/Hasher/
rm -rf $(pwd)/deployment/build/hasher
dotnet publish $(pwd)/src/Hasher/ -o $(pwd)/deployment/build/hasher -c Release