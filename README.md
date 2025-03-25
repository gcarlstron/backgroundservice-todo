# Project Wiki: BackgroundService Todo Application
This wiki documents the creation process of a .NET worker service that monitors tasks and notifies about expired ones.

## Project Setup
1. Create the Worker Projec
   ```
   dotnet new worker -n backgroundservice-todo
   cd backgroundservice-todo
   
2. Add Required Packages
   ```
   dotnet add package Microsoft.EntityFrameworkCore.SqlServer

## Project Structure
1. Create Model Class
Create Models/MyTask.cs:
```
  namespace backgroundservice_todo.Models;
  
  public class MyTask
  {
      public string Id { get; set; }
      public string Title { get; set; }
      public string Description { get; set; }
      public DateTime DueDate { get; set; }
  }
```
2. Create Database Context
Create Data/TaskContext.cs:
```
using backgroundservice_todo.Models;
using Microsoft.EntityFrameworkCore;

namespace backgroundservice_todo.Data;

public class TaskContext(DbContextOptions<TaskContext> options) : DbContext(options)
{
    public DbSet<MyTask> MyTasks { get; set; }
}
```
3. Create the Task Service
Create Service/MyTaskService.cs:
```
using backgroundservice_todo.Service;

namespace backgroundservice_todo;

public class Worker(ILogger<Worker> logger, MyTaskService myTaskService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            myTaskService.ProcessTasks();
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
```
5. Configure Program.cs
Update Program.cs:
```
using backgroundservice_todo;
using backgroundservice_todo.Data;
using backgroundservice_todo.Service;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContextFactory<TaskContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TaskContext")));

builder.Services.AddSingleton<MyTaskService>();

var host = builder.Build();
host.Run();
```
6. Configure Connection String
Update appsettings.json and appsettings.Development.json:
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "ConnectionStrings": {
    "TaskContext": "Server=localHost;Database=master;User Id=sa;Password=!Parola55;TrustServerCertificate=True;"
  }
}
```

## Database Setup
1. Create Database Table
Create a SQL script for the MyTask table:

```
CREATE TABLE MyTask (
    Id NVARCHAR(450) PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    DueDate DATETIME NOT NULL
);
```

2. Create the Stored Procedure
Create the stored procedure for expired tasks:
```
CREATE PROCEDURE [dbo].[EnviarNotificacaoTarefaVencida]
AS
BEGIN
    SELECT
        Id,
        Title,
        Description,
        DueDate
    FROM MyTask
    WHERE CONVERT(DATE, DueDate) < CONVERT(DATE, GETDATE());
END
```
