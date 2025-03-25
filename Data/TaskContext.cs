using backgroundservice_todo.Models;
using Microsoft.EntityFrameworkCore;

namespace backgroundservice_todo.Data;

public class TaskContext(DbContextOptions<TaskContext> options) : DbContext(options)
{
    public DbSet<MyTask> MyTasks { get; set; }
}