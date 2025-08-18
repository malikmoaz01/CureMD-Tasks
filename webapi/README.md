# PVMS - 6606

## Introduction
PVMS (Project Version Management System) - 6606 is developed using **.NET 8** and incorporates various packages to ensure secure authentication, configuration handling, logging, and API documentation.

---

## 🚀 Prerequisites

Before running the project, please make sure you:

- Have [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) installed.
- Navigate to the `Assignment 5` folder.
- Run the provided SQL scripts (`DDL` and `SPs`) **one by one** to set up your database schema and stored procedures.

---

## 📦 Required NuGet Packages

Install the required packages using the following command (individually or in a `.csproj` file):

```bash
dotnet add package BCrypt.Net-Next --version 4.0.3
dotnet add package FluentValidation.AspNetCore --version 11.3.1
dotnet add package Microsoft.AspNetCore.App --version 2.2.8
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.8
dotnet add package Microsoft.AspNetCore.Mvc.Core --version 2.3.0
dotnet add package Microsoft.Data.SqlClient --version 6.0.2
dotnet add package Microsoft.Extensions.Configuration --version 9.0.8
dotnet add package Microsoft.Extensions.Configuration.Abstractions --version 9.0.8
dotnet add package Microsoft.Extensions.Configuration.Binder --version 9.0.8
dotnet add package Microsoft.Extensions.Configuration.Json --version 9.0.8
dotnet add package Microsoft.Extensions.DependencyInjection --version 9.0.8
dotnet add package Microsoft.IdentityModel.Tokens --version 8.13.1
dotnet add package Serilog.AspNetCore --version 9.0.0
dotnet add package Serilog.Sinks.Console --version 6.0.0
dotnet add package Serilog.Sinks.File --version 7.0.0
dotnet add package Swashbuckle.AspNetCore --version 9.0.3
dotnet add package System.Data.SqlClient --version 4.9.0
dotnet add package System.IdentityModel.Tokens.Jwt --version 8.13.1
````

---

## ⚙️ Configuration Instructions

1. **Google App Password (for Forgot Password functionality):**
   Update the `appsettings.json` file with your Google App password.

2. **Database Connection String:**
   Modify the connection string in `appsettings.json` according to your local SQL Server setup.

---

## ▶️ Run the Application

Once everything is set up:

```bash
dotnet run
```

Use the browser or tools like Postman to test endpoints.

---

## ⚠️ Important Notes

* **Referential Integrity:**
  Records that are part of referential integrity constraints **will not be deleted**. Handle such cases carefully.

* **Testing Password Recovery:**
  The email-based password recovery system requires a valid Google App password configured in your settings.

---

## 📝 Author's Note

Before testing or checking the project:

* Ensure the database scripts (DDL & SPs) are executed.
* Confirm all configuration settings are correct.
* Then run and explore the functionality.
