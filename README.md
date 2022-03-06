# Project-NFC-M2


### Prerequisites

- Docker et Docker-compose (Docker-Desktop pour windows) : https://www.docker.com/products/docker-desktop
- SDK dotnet 6 : https://dotnet.microsoft.com/en-us/download/dotnet/6.0

### Start server

- cd ./NFChoes
- docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d

### Start Console App

- cd ./NFConsoleApp
- dotnet build --configuration Release
- ./NFConsoleApp/bin/Release/net6.0/NFConsoleApp.exe


