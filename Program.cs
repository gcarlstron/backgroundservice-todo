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