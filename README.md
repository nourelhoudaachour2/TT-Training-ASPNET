# Training Tunisie Télécom

Application ASP.NET Core MVC de gestion des formations (employés, formateurs, sessions, présences, reporting).

## Stack
- ASP.NET Core MVC, Entity Framework Core
- SQL Server

## Lancer le projet
1. `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<votre chaîne>"`
2. `dotnet ef database update`
3. `dotnet run`

## Modules
Employés, Formations, Sessions, Présences & justifications, Reporting, Portail employé.
