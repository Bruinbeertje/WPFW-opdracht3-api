# WPFW opdracht 3

Backend-API voor mijn portfoliosite met projecten en blogposts.

## Techniek
- .NET-versie: 10
- API-stijl: controllers
- Codestructuur:

```
Controllers/        HTTP: routes en statuscodes
    ProjectsController
    BlogPostsController
Services/           logica en mapping entiteit ↔ DTO
    IProjectService, ProjectService
    IBlogPostService, BlogPostService
Dtos/               wat de client stuurt en terugkrijgt
    ProjectDtos
    BlogPostDtos
Data/               entiteiten en DbContext
    Entities/
        Project.cs
        BlogPost.cs
    PortfolioDb
Migrations/         databaseschema (InitialCreate)
Program.cs          registratie van services en de pipeline
```

## Database
- SQL Server: de connectionstring staat in `appsettings.json` onder `DefaultConnection`:
  `Server=.\SQLEXPRESS;Database=PortfolioDb;Trusted_Connection=True;TrustServerCertificate=True;`
  - Heet jouw server of database anders? Verander dan de waarde achter `Server=` of `Database=`.
  - `Trusted_Connection=True` is Windows Authentication, dus je hebt geen wachtwoord nodig.
- Aanmaken: met een migratie. Daarmee kan ik het databaseschema later bijwerken, bijvoorbeeld bij een nieuw veld in een tabel.

## Starten
Vereist: .NET 10 SDK en SQL Server.

1. Pas zo nodig de connectionstring aan (zie Database).
2. Maak de database aan: `dotnet ef database update` (of `Update-Database` in de Package Manager Console).
3. Start de app in de map met het `.csproj`: `dotnet run --launch-profile https`
4. Open Swagger: https://localhost:7175/swagger

Met F5 in Visual Studio opent Swagger automatisch in je browser (ingesteld in `launchSettings.json`).

## Endpoints

| Methode | Route | Wat het doet |
|---|---|---|
| GET | /api/projects | Alle projecten ophalen (200) |
| GET | /api/projects/{id} | Eén project ophalen (200, of 404 als het niet bestaat) |
| POST | /api/projects | Een project aanmaken (201) |
| PUT | /api/projects/{id} | Een project wijzigen (204, of 404) |
| DELETE | /api/projects/{id} | Een project verwijderen (204, of 404) |
| GET | /api/blogposts | Alle blogposts ophalen (200) |
| GET | /api/blogposts/{id} | Eén blogpost ophalen (200, of 404 als hij niet bestaat) |
| POST | /api/blogposts | Een blogpost aanmaken (201) |
| PUT | /api/blogposts/{id} | Een blogpost wijzigen (204, of 404) |
| DELETE | /api/blogposts/{id} | Een blogpost verwijderen (204, of 404) |
