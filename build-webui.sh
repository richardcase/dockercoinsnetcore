#!/bin/bash
set -e
dotnet restore $(pwd)/src/Webui/
rm -rf $(pwd)/deployment/build/webui
dotnet publish $(pwd)/src/Webui/ -o $(pwd)/deployment/build/webui -c Release