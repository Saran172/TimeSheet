# MyApp
A Blazor Server app for managing employee work logs and tracking project hours etc.

## Features
- Log daily work with start/end time
- Validate overlapping time entries (past and current days)
- Customer & Project selection with dynamic filtering
- Forgot Password flow with security question
- Notifications sent daily at 6 PM (Disabled)
- Displays Screens based on role
- CRUD operation for Customer, Project, Employee
- Export to multiple file types : available in most of the screens

## Tech Stack
- .NET 8 (Blazor Server)
- SQL Server (EF Core) and ADO.NET
- DevExpress Components

## Setup
1. Clone repo
2. Run `dotnet restore`
3. Update `appsettings.json` with DB connection string
4. Run migrations: `dotnet ef database update`
5. Launch with `dotnet run`
