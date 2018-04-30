# SampleMicroserviceApplication

To run:

0. Make sure you have .NET Core SDK 2.0.3 .NET Core SDK 1.1.9 installed (https://www.microsoft.com/net/download/all) and clone the repo.
1. Setup the databases for LibraryCatalog and BookRequest projects on LocalDb by running the .sql files in their respective DatabaseScripts folders. 
2. Update the connection strings in LibraryCatalog and BookRequest EnvironmentVariables.cs classes.
3. Open Powershell and run the .start-app.ps1.  There should be 3 command windows that open up.
4. Open Chrome and navigate to http://localhost:5000/1
5. If it doesn't work, send me a message on Quadrus Slack (or Quadrus Email).
