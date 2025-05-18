#!/bin/bash
sleep 10 # ensure db process has fully started
dotnet publish -c Release -o out
dotnet ef database update -v
dotnet out/Video\ Library\ Api.dll