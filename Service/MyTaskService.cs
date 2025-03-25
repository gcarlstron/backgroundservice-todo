using backgroundservice_todo.Data;
using Microsoft.EntityFrameworkCore;

namespace backgroundservice_todo.Service;

public class MyTaskService(IDbContextFactory<TaskContext> contextFactory)
{
    public void ProcessTasks()
    {
        using var context = contextFactory.CreateDbContext();
        var expiredTasks = context.MyTasks
            .FromSqlRaw("EXEC EnviarNotificacaoTarefaVencida")
            .AsNoTracking()
            .ToList();

        foreach (var task in expiredTasks)
        {
            Console.WriteLine($"Tarefa {task.Title} (ID: {task.Id}) venceu hoje. Descrição: {task.Description}");
        }
    }
}