#!/bin/sh
echo ASPNETCORE_ENVIRONMENT is $ASPNETCORE_ENVIRONMENT
echo WEBHOOKS_CSPROJ is $WEBHOOKS_CSPROJ

dotnet ef --version
dotnet ef database update --verbose --project $WEBHOOKS_CSPROJ --context WebhooksContext
