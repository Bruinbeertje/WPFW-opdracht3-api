# WPFW opdracht 4

Backend-API voor mijn portfoliosite met projecten en blogposts, uitgebreid met ASP.NET Core Identity (registreren, inloggen, rollen) en xUnit-tests via HTTP.

## Techniek
- .NET-versie: 10
- API-stijl: controllers
- Inloggen: ASP.NET Core Identity met een authenticatiecookie
- Testen: xUnit met `WebApplicationFactory` (aparte testdatabase)
- Codestructuur:

```
WPFW opdracht/
    Controllers/        HTTP: routes, statuscodes en toegang ([Authorize]/[AllowAnonymous])
        ProjectsController
        BlogPostsController
        AuthController
    Services/           logica en mapping entiteit ↔ DTO
        IProjectService, ProjectService
        IBlogPostService, BlogPostService
        IAuthService, AuthService
    Dtos/               wat de client stuurt en terugkrijgt
        ProjectDtos
        BlogPostDtos
        AuthDtos
    Data/               entiteiten en DbContext
        Entities/
            Project.cs
            BlogPost.cs
            User.cs
        PortfolioDb     (IdentityDbContext<User>)
    Migrations/         databaseschema (InitialCreate, AddIdentity)
    Program.cs          registratie van services, Identity, policy, rollen en de pipeline
    appsettings.Testing.json   connectionstring van de testdatabase
WPFW.Tests/             xUnit-testproject
    TestApiFactory      start de API in de omgeving "Testing"
    ProjectsApiTests
    BlogPostsApiTests
    AuthApiTests
scripts/
    assign-role.sql     rol Editor of Admin lokaal toekennen
```

## Database
- SQL Server: de connectionstring staat in `appsettings.json` onder `DefaultConnection`:
  `Server=.\SQLEXPRESS;Database=PortfolioDb;Trusted_Connection=True;TrustServerCertificate=True;`
  - Heet jouw server of database anders? Verander dan de waarde achter `Server=` of `Database=`.
  - `Trusted_Connection=True` is Windows Authentication, dus je hebt geen wachtwoord nodig.
- Aanmaken: met migraties (`InitialCreate` en `AddIdentity`). De migratie `AddIdentity` maakt de Identity-tabellen aan, zoals `AspNetUsers`, `AspNetRoles` en `AspNetUserRoles`.
- Bij het opstarten voert de app de migraties zelf uit en maakt hij de rollen **Gast**, **Editor** en **Admin** aan als die nog niet bestaan. Er worden geen gebruikers of wachtwoorden aangemaakt.

## Starten
Vereist: .NET 10 SDK en SQL Server.

1. Pas zo nodig de connectionstring aan (zie Database).
2. Maak de database aan: `dotnet ef database update` (of `Update-Database` in de Package Manager Console). Dit mag je ook overslaan: de app migreert bij het opstarten zelf.
3. Start de app in de map met het `.csproj`: `dotnet run --launch-profile https`
4. Open Swagger: https://localhost:7175/swagger

Met F5 in Visual Studio opent Swagger automatisch in je browser (ingesteld in `launchSettings.json`).

## Lokale gebruikers en rollen
Er staan geen wachtwoorden in deze repository. Je maakt de gebruikers zelf aan:

1. Registreer drie accounts via `POST /api/auth/register` in Swagger, bijvoorbeeld `gast@example.local`, `editor@example.local` en `admin@example.local`. Kies zelf een wachtwoord (minimaal 6 tekens, met een hoofdletter, kleine letter, cijfer en leesteken) en bewaar het alleen lokaal.
2. Elke nieuwe gebruiker krijgt automatisch de rol **Gast**.
3. Geef de Editor en de Admin hun rol met `scripts/assign-role.sql`: open het script in SSMS op de database `PortfolioDb`, vul het e-mailadres en de rol (`Editor` of `Admin`) in en voer het uit.
4. **Log daarna opnieuw in** met dat account. De rollen worden bij het inloggen in de cookie gezet, dus een rolwijziging telt pas na een nieuwe login.

In Swagger ben je steeds de laatste gebruiker die inlogde. Met `POST /api/auth/logout` log je uit.

## Endpoints

| Methode | Route | Wat het doet | Wie mag dit |
|---|---|---|---|
| GET | /api/projects | Alle projecten ophalen (200) | Iedereen |
| GET | /api/projects/{id} | Eén project ophalen (200, of 404 als het niet bestaat) | Iedereen |
| POST | /api/projects | Een project aanmaken (201, of 400 bij ongeldige invoer) | Editor, Admin |
| PUT | /api/projects/{id} | Een project wijzigen (204, of 400 / 404) | Editor, Admin |
| DELETE | /api/projects/{id} | Een project verwijderen (204, of 404) | Editor, Admin |
| GET | /api/blogposts | Alle blogposts ophalen (200) | Iedereen |
| GET | /api/blogposts/{id} | Eén blogpost ophalen (200, of 404 als hij niet bestaat) | Iedereen |
| POST | /api/blogposts | Een blogpost aanmaken (201, of 400 bij ongeldige invoer) | Editor, Admin |
| PUT | /api/blogposts/{id} | Een blogpost wijzigen (204, of 400 / 404) | Editor, Admin |
| DELETE | /api/blogposts/{id} | Een blogpost verwijderen (204, of 404) | Editor, Admin |
| POST | /api/auth/register | Account aanmaken met rol Gast (200, of 400 bij bestaand e-mailadres of zwak wachtwoord) | Iedereen |
| POST | /api/auth/login | Inloggen, geeft een authenticatiecookie terug (200, of 401 bij fout wachtwoord) | Iedereen |
| POST | /api/auth/logout | Uitloggen, verwijdert de cookie (200) | Iedereen |

Schrijven zonder login geeft **401 Unauthorized**. Schrijven als ingelogde gebruiker zonder rol Editor of Admin (dus als Gast) geeft **403 Forbidden**.

## Testen
De tests staan in het project `WPFW.Tests` en sturen echte HTTP-requests naar de API via `WebApplicationFactory`.

- De tests draaien in de omgeving `Testing` en gebruiken de database `PortfolioDb_Test` uit `appsettings.Testing.json`. Je gewone `PortfolioDb` wordt niet aangeraakt.
- De testdatabase wordt bij de eerste test automatisch aangemaakt.
- Testgebruikers en hun wachtwoorden worden tijdens de test met een `Guid` gemaakt, dus er staan geen wachtwoorden in de code.

Draaien in de map van de solution:

```
dotnet test
```

Of in Visual Studio: Test → Test Explorer → Run All.
