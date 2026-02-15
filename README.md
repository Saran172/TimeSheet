# TimeSheet

A modern **.NET-based Timesheet Application** for tracking work hours, tasks, projects, and generating reports.

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0+-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET">
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="C#">
  <img src="https://img.shields.io/github/repo-size/Saran172/TimeSheet?style=for-the-badge" alt="Repo Size">
  <img src="https://img.shields.io/github/last-commit/Saran172/TimeSheet?style=for-the-badge" alt="Last Commit">
</p>

## ✨ Features 

- Log daily work with start/end time
- Validate overlapping time entries (past and current days)
- Customer & Project selection with dynamic filtering
- Forgot Password flow with security question
- Notifications sent daily at 6 PM (Disabled)
- Displays Screens based on role
- CRUD operation for Customer, Project, Employee
- Export to multiple file types : available in most of the screens

## 🚀 Tech Stack

| Category          | Technology                  |
|-------------------|-----------------------------|
| Backend           | .NET 8                      |
| Frontend          | HTML / CSS / JavaScript     |
| Database          | SQL Server                  |
| ORM               | Entity Framework Core       |
| Authentication    | ASP.NET Identity / JWT      |
| UI Framework      | Razor Pages / Blazor / MVC  |
| IDE               | Visual Studio               |

## 📋 Getting Started

### Prerequisites

- [.NET 8+ SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022+ (or VS Code with C# Dev Kit)

### Clone & Run

```bash
# Clone the repository
git clone https://github.com/Saran172/TimeSheet.git
cd TimeSheet/Task-1

# Restore dependencies
dotnet restore Task-1.sln

# Build the solution
dotnet build Task-1.sln

# Run the application
dotnet run --project Task-1/Task-1.csproj   # Adjust project name/path if needed
