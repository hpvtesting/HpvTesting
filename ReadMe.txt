This Project is generated from Solution Generator. This Project is included AutoMapping, Mail Configuration, Background jobs,and Logging.

This project has generated API Controllers with Repository and Service With its Interfaces and Also have UoW it have completely with reference. This Project Also generated Selected Table's
Database Models. Need To Change some Codes Before Start The Project Follow Below Steps. 

Step 1 : restore nuget packages.

Step 2 : Chnage Database Connection String in "appsetting.json" and "Devlopment.appsetting.json" File.
set "//" if contain "/". 

Step 3 : Set Database Model Filds With your Database table Filds.

Step 4 : Press Ctrl+k+d In "IUnitOfWork" and "UnitOfWork" and "Startup.cs". 

Note : all services and repositorys and it's interfaces are need to configure in strtup.cs file in "RegisterServices" and "RegisterRepositories" method in API. 

Other change accourding to your requirements.