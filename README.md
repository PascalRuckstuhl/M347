# M347 Aufgaben 7.4-1 & 7.4-2

## Inhaltsverzeichnis

- [0. Vorwort](#0. Vorwort)
- [1. Basis](#1. Basis)
	- [1.1 Zwischenergebnis](#1.1 Zwischenergebnis)
- [2. Multistage Image](#2. Multistage Image)
	- [2.1 docker-compose.yml](#2.1 docker-compose.yml)
	- [2.2 Dockerfile](#2.2 Dockerfile)
	- [2.3 Zwischenergebnis](#2.3 Zwischenergebnis)
- [3. MongoDB](#3. MongoDB)
	- [3.1 MongoDB-Container](#3.1 MongoDB-Container)
	- [3.2 Program.cs](#3.2 Program.cs)
- [4. Endresultat](#4. Endresultat)

# 0. Vorwort 

Dieses Projekt verwendet dotnet 6.0 also falls es nicht bereits installiert ist, kann man es mit den Folgenden Commands herunterladen.
````nginx
sudo su
apt update
apt install dotnet-sdk-6.0
````



Falls Sie GitHub verwenden, ist ein '.gitignore'-File erwünscht, um ein sauberes Repository zu gewährleisten. Dafür müssen Sie nur den Folgenden Command in dem **Project_folder** ausführen.
````
dotnet new gitignore
````




# 1. Basis

Erstelle den Ordner **Project_folder** in dem das Projekt aufgebaut wird.

````
mkdir Project_folder
````



Jetzt kann das .NET Projekt, welches ich **WebApi** nenne, mit einer web template erstellt werden.

````
cd Project_folder
dotnet new web --name WebApi
````



Ersetze den Inhalt der Datei **"/Project_folder/WebApi/Properties/launchSettings.json"** mit dem folgenden Inhalt Code.

````json
{
    "profiles": {
        "WebApi": {
            "commandName": "Project",
            "dotnetRunMessages": true,
            "launchBrowser": true,
            "applicationUrl": "http://localhost:5001",
            "environmentVariables": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            }
            
        }
    }
}
````

#### 1.1 Zwischenergebnis

Nun ist die Anwendung einfach erreichbar indem man in dem **/Project_folder/WebApi** Verzeichnis den Befehl <dotnet run> ausführt und im Browser "http://localhost:5001" ansteuert.

mit "Ctrl+C" kann man die Anwendung beenden.

---

# 2. Multistage Image

#### 2.1 docker-compose.yml

Erstelle im Verzeichnis **/Project_folder** ein File namens **"docker-compose.yml"** und öffne es in VSC.

````
touch docker-compose.yml
code docker-compose.yml
````

Dann kopieren Sie diesen Code in **"docker-compose.yml"** ein.

````yaml
version: "3.9"
services:
  webapi:
    build: WebApi
    ports:
     - 5001:5001
````

###### Beschreibung

Die **"docker-compose.yml"**-Datei wird verwendet um die Konfigurationen und das Starten von Anwendungen mit Docker-Compose-Kommandos zu definieren.

#### 2.2 Dockerfile

Erstelle im Verzeichnis **WebApi** ein File namens **"Dockerfile"** und öffne es in VSC.

````
touch Dockerfile
code Dockerfile
````

Kopiere den den untenstehenden Code und füge ihn **"Dockerfile"** ein.

````dockerfile
# 1. Build compile image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /build
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o out

# 2. Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /build/out .
ENV ASPNETCORE_URLS=http://*:5001
EXPOSE 5001
ENTRYPOINT ["dotnet", "WebApi.dll"]
````



###### Beschreibung

Das **"Dockerfile"** ist dafür zuständig ein Image einer Docker-Anwendung zu erstellen und definiert die notwendigen Komponenten, Abhängigkeiten und dessen Konfigurationen. 

In diesem **"Dockerfile"** werden zwei Container erstellt; den ersten Container der den Code für den zweiten Container kompiliert und diesen übergibt. und der Zweite der den Erhaltenen Code ausführt.

Der Vorteil an dem ist, dass der zweite Container viel kleiner ist da der Compiler abgekoppelt ist.

#### 2.3 Zwischenergebnis

Nun ist das Projekt ausführbar mittels docker compose. 
Um Änderungen zu verwenden und das Projekt starten zu können muss folgender Command ausgeführt werden;

```
docker compose up --build
```

Wenn Es bereits einmal Kompiliert wurde kann man es einfach mit diesem Command starten.
````
docker compose up
````

beenden kann man es mit "Ctrl+C" und diesem Command.

````
docker compose down
````



---

# 3. MongoDB



#### 3.1 MongoDB-Container

Im **"WebApi"** Folder ist der **"MongoDB.Driver"** zu hinzufügen.

````
dotnet add package MongoDB.Driver
````



Ersetzen Sie **"/Project_folder/docker-compose.yml"** mit folgenden Code.

````yaml
version: "3.9"
services:
  webapi:
    build: WebApi
    ports:
      - 5001:5001


  mongodb:
    image: mongo
    volumes:
     - mongoData:/data/db
volumes:
  mongoData:

````

###### Beschreibung


Der hinzugefügte Code macht, dass ein MongoDB-Container erzeugt und verbunden wird. 

#### 3.2 Program.cs

In **"/Project_folder/WebApi/Program.cs"** ist der bestehende Code mit diesem zu ersetzen.

````csharp
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/check", () => {
try
{

    var mongoClient = new MongoClient("mongodb://mongodb:27017");
    var databaseNames = mongoClient.ListDatabaseNames().ToList();

    return "Zugriff auf MongDB ok." + string.Join(",", databaseNames);
}
catch (System.Exception e)
{
    return "Zugriff auf MongoDB funktioniert nicht: " + e.Message;
}
});

app.Run();
````

###### Beschreibung

Diese neuen Zeilen Code sind den Inhalt von der Seite **"/check"**.



# 4. Endresultat

Nun ist das Projekt komplett ausführbar man muss nur folgende Commands im Verzeichnis **"Project_folder"** verwenden

Um Änderungen zu anzuwenden und das Projekt zu starten

```
docker compose up --build
```

Wenn Es bereits einmal Kompiliert wurde zu starten.

````
docker compose up
````

beenden kann man es mit "Ctrl+C" und diesem Command.

````
docker compose down
````



