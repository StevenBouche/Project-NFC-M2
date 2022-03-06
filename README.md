# Project-NFC-M2


### Prerequisites

- Docker et Docker-compose (Docker-Desktop pour windows) : https://www.docker.com/products/docker-desktop
- SDK dotnet 6 : https://dotnet.microsoft.com/en-us/download/dotnet/6.0

### Start server

- cd ./NFChoes
- docker-compose -f docker-compose.yml -f docker-compose.override.yml build
- docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d

### Start Console App

- cd ./NFConsoleApp
- dotnet build --configuration Release
- ./NFConsoleApp/bin/Release/net6.0/NFConsoleApp.exe

### Start NFCFront

Pour voir en temps réel le nombre de personnes dans chaque magasins

- cd ./NFCFront
- npm i
- npm start

### App mobile


APK FILE : https://drive.google.com/file/d/1o8DWQweJhGwNsvT3EemXkCi5PC6CvyS0/view?usp=sharing

### Use App

Une fois les autres commandes exécutées et que NFConsoleApp est lancée vous pouvez rensigner l'userid que vous souhaitez utiliser.

Ici on va utiliser l'id : "mbds".

#### 1 - 

![Alt text](readme-img/consoleApp1.png?raw=true "Console App")

#### 2 - 
Lancer l'application et cliquer sur WRITE TAG

![Alt text](readme-img/app-1-3.jpg?raw=true "WRITE TAG")

#### 3 - 
Renseigner l'userid et cliquer sur LINK ID TO NFC TAG ensuite placez la carte sur votre smartphone. Si une vibration a lieu l'opétation a reussie

![Alt text](readme-img/app-2.jpg?raw=true "WRITE TAG")

#### 4 - 
Revenir sur le menu et cliquer sur READ TAG

![Alt text](readme-img/app-1-3.jpg?raw=true "READ TAG")

#### 5 - 
Renseigner l'adresse du serveur mqtt et cliquer sur CONNECT TP MQTT
(faire ipconfig sur le pc où NFChoes a été lancé)
Si la connection réussie vous pouvez cliquer sur SCAN TAG. Une fois le bouton pressé vous pouvez placer votre tag, un message sera alors envoyé au serveur.

![Alt text](readme-img/app-4.jpg?raw=true "SCAN TAG")


#### 6 - 
ConsoleApp va alors afficher un retour

![Alt text](readme-img/consoleApp2.png?raw=true "ConsoleApp")





