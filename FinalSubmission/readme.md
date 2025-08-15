# .NET Core Web API Assignment - Patient Visit Manager


## Prerequisites

* .NET SDK (Latest LTS version) -> (Kindly dont use 7 version )
* SQL Server or any compatible relational database
* Postman 

---

## Setup Instructions

1. **Database Setup:**

   * Before running the application, **execute all DDL scripts and stored procedures** provided with the project to create the required database schema and objects.
   * Update the connection string in `appsettings.json` to point to your own database instance.

2. **Connection String Configuration & jwt token you can use :**

   ```json
   "ConnectionStrings": {
     "DefaultConnection": ""
   }
   ```

3. **Running the Application:**

   * The Web API runs on **localhost port 5000** by default. Make sure this port is free or update accordingly.
   * Launch the API with:

     ```
     dotnet run
     ```
   * Open your browser and access the UI pages at:
     `http://localhost:5000`

4. **Testing the API:**

   * You can test the API either via the provided HTML front-end UI or use the Postman collection (included in the repo) to test all endpoints and authentication flows.

---

## Notes

* I reviewed submission detail today , so **no JSON format for this submissions** is available.
* Kindly install and use the **latest versions** of all dependencies and SDKs.
* **Dont run DML Script because i hashed my password so it will fail login**
* Please **run the DDL scripts and stored procedures before running the code**.
* Change the connection string to match your own database setup.
* The localhost is set to run on port 5000, please use `http://localhost:5000` for testing.
* You can test the project using the UI provided in the `wwwroot` folder or using Postman.

---
