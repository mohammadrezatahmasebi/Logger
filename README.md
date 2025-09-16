# .NET Audit Logging Solution

This solution contains a .NET class library for audit logging and an example web API project that demonstrates its usage.

## Projects

The solution contains the following projects:

-   **`AuditLogging`**: A .NET class library that provides middleware and services for auditing HTTP requests and responses. It is designed to be used as a NuGet package. For detailed instructions on how to use this library, see the [`AuditLogging/README.md`](AuditLogging/README.md) file.
-   **`Logger`**: An ASP.NET Core web API project that serves as an example of how to use the `AuditLogging` library. It is configured to use MongoDB for persistence and includes controllers for retrieving audit logs.
-   **`AuditLogging.Tests`**: A unit test project for the `AuditLogging` library.

## How to Run the Example

To run the example `Logger` application, you will need:

-   .NET 8 SDK
-   A running MongoDB instance

### 1. Configure the Connection String

The `Logger` application reads the MongoDB connection string from `Logger/appsettings.json`. Make sure the `MongoDbSettings` section is configured correctly for your environment.

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "AuditDb"
  }
}
```

### 2. Run the Application

You can run the `Logger` application using the .NET CLI:

```bash
dotnet run --project Logger/Logger.csproj
```

The API will be available at `http://localhost:5000` (or as configured in `launchSettings.json`).

### 3. Use the API

You can use the following endpoints to test the application:

-   `GET /weatherforecast`: This will trigger a simple request that will be audited.
-   `GET /test-logging`: This will log a custom message with the same trace context as the request audit.
-   `GET /audit-logs/{correlationId}`: Retrieve logs for a specific correlation ID.
-   `GET /audit-logs`: Retrieve all logs, grouped by correlation ID.
