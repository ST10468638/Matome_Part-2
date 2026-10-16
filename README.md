# RaceDay Part 2 — Event Management API

## 1. Project Overview

RaceDay Part 2 is an ASP.NET Core Web API developed in C# using .NET 8. The application supports the management of running, walking, and cycling events.

The API uses Entity Framework Core with SQL Server for data persistence. It provides session-based authentication, role-based authorization, event and category management, participant enrolments, race results, and event route management.

The API can be explored and tested through Swagger UI.

## 2. Technologies Used

- C# and ASP.NET Core Web API (.NET 8)
- Entity Framework Core 8
- Microsoft SQL Server
- Entity Framework Core Code-First migrations
- Swagger / OpenAPI using Swashbuckle
- xUnit for automated testing
- ASP.NET Core integration testing
- Entity Framework Core InMemory provider for automated tests

## 3. User Roles

### Organiser

An Organiser can:

- Register using the configured Organiser invitation code.
- Log in and manage their own profile.
- Create, view, update, and delete their own events.
- Create, view, update, and delete categories for events they own.
- Create, view, update, and delete routes for events they own.
- View enrolments for their own events.
- Record race results for participants enrolled in their events.

### Participant

A Participant can:

- Register and log in.
- View and update their own profile.
- View events and event categories.
- Enrol in an available category.
- View their own enrolments.
- View their own race results.

The API applies session authentication and role-based authorization to protected endpoints.

## 4. Main Features

### Authentication and Profiles

- User registration and login.
- Password hashing using ASP.NET Core's password hashing implementation.
- Session-based authentication.
- Role-based endpoint protection.
- Viewing and updating the authenticated user's profile.
- Logout functionality.

### Event Management

Events include a name, description, event type, event date, venue, city, province, registration dates, and an optional maximum participant limit.

Supported event types are:

- Running
- Walking
- Cycling

Organisers can manage only their own events.

### Category Management

Categories are associated with events and support two types:

- **Age:** Uses minimum and maximum ages.
- **Distance:** Uses a distance measured in kilometres.

Categories also include a name, description, entry fee, and capacity. The API validates category details and prevents duplicate category names within the same event.

### Event Route Management

An event can have one stored route containing:

- Route name
- Distance in kilometres
- Start location
- Finish location
- Optional route URL

Only the Organiser who owns an event can create, update, or delete its route. Authenticated users can view an event's route.

### Participant Enrolments

Participants can enrol in categories when registration is open.

The API checks that the category exists, registration dates permit enrolment, the participant has not already enrolled in that category, and the category and event capacity limits have not been reached.

Organisers can view enrolments for their own events, while Participants can view their own enrolments.

### Race Results

Organisers can record race results for enrolments associated with events they own.

Results support finish time, overall position, category position, and result status.

Supported result statuses are:

- Finished
- DNF
- DNS
- Disqualified

Participants can view their own results.

## 5. Project Structure

The Part 2 project is organised into an API project and a separate test project.

- **RaceDay .Api:** Contains the Web API, controllers, models, DTOs, filters, database context, configuration, and EF Core migrations.
- **RaceDay.Api.Tests:** Contains automated tests, integration test configuration, and the custom WebApplicationFactory.

The API uses dependency injection to configure its database context, session services, authorization, controllers, and Swagger.

## 6. Prerequisites

Install the following before running the project:

- .NET 8 SDK
- Microsoft SQL Server or SQL Server Express
- Visual Studio 2022 or another compatible .NET development environment
- Git, if working with the GitHub repository

## 7. Database Configuration

The application uses a SQL Server connection string named `RaceDayConnection`.

Open the API project's `appsettings.json` file and configure the connection string for your SQL Server instance.

Example:

```json
{
  "ConnectionStrings": {
    "RaceDayConnection": "Server=YOUR_SQL_SERVER\\SQLEXPRESS;Database=RaceDayPart2DB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```

Replace `YOUR_SQL_SERVER` and the instance name with the correct values for your local SQL Server installation.

### Organiser Invitation Code

Organiser registration requires a configured invitation code. Configure `Registration:OrganiserInviteCode` using local development settings or .NET User Secrets.

From the Part 2 folder, you can set a development secret with:

```powershell
dotnet user-secrets set "Registration:OrganiserInviteCode" "YOUR-STRONG-INVITE-CODE" --project "RaceDay .Api/RaceDay .Api.csproj"
```

Replace the example value with your own development invitation code. Do not commit private production secrets to the repository.

## 8. Restore, Build, and Run

Open a terminal in the Part 2 folder.

### Restore dependencies

```powershell
dotnet restore "RaceDay .Api/RaceDay .Api.csproj"
```

### Build the API

```powershell
dotnet build "RaceDay .Api/RaceDay .Api.csproj"
```

### Apply database migrations

Alternatively, in Visual Studio, open **Tools → NuGet Package Manager → Package Manager Console** and set the API project as the Default project.

Run:

```powershell
Update-Database
```

### Run the API

```powershell
dotnet run --project "RaceDay .Api/RaceDay .Api.csproj"
```

After the API starts, open the HTTPS localhost address shown in the terminal and append `/swagger`.

For example:

```text
https://localhost:YOUR_PORT/swagger
```

Replace `YOUR_PORT` with the port displayed by the application.

## 9. Testing

The automated tests are contained in the `RaceDay.Api.Tests` project.

Run all tests from the Part 2 folder with:

```powershell
dotnet test "RaceDay.Api.Tests/RaceDay.Api.Tests.csproj" --verbosity normal
```

The test suite covers registration, login, authentication, authorization, profile operations, event and category management, event routes, participant enrolments, duplicate prevention, race results, and ownership restrictions.

The automated tests use an in-memory database through a custom test factory. This allows the API's test scenarios to run without writing test records to the configured SQL Server database.

## 10. Main API Endpoints

The RaceDay API provides endpoints for user registration and authentication, profile management, event and category management, event routes, participant enrolments, and race results. Access to protected endpoints is controlled through session-based authentication and role-based authorization to ensure that Organisers and Participants can perform only the actions permitted for their roles. Swagger UI provides interactive API documentation where users can explore and test the available endpoints.

## 11. Database Migrations

The project includes EF Core migrations for creating the initial database schema and updating the user email and category configuration.

The migrations currently included in the project are:

- `InitialCreate`
- `UpdateEmailUniqueIndex`
- `AddCategoryTypesAndDistance`
- `ChangeCategoryNameToUniqueIndex`

Apply the migrations to a correctly configured SQL Server database before using the API with that database.

## 12. Security and Validation

- Passwords are stored as hashes, not plain-text passwords.
- Protected endpoints require session authentication.
- Organiser operations check ownership of the affected event.
- Category and enrolment operations validate relevant constraints.
- The API uses validation attributes and database constraints to protect data integrity.
- Connection strings and private invitation codes should be configured appropriately for the deployment environment.

## 13. GitHub Actions Workflow

[View the RaceDay GitHub Actions Workflow](PASTE_WORKFLOW_LINK_HERE)

## 14. YouTube Demonstration Video

[Watch the RaceDay Part 2 Demonstration](PASTE_YOUTUBE_LINK_HERE)
