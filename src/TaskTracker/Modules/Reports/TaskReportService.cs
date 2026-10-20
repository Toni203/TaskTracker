using System.Text;
using TaskTracker.Models;
using TaskTracker.Services;

namespace TaskTracker.Modules.Reports;

public class TaskReportService
{
    private readonly TaskService _taskService;

    public TaskReportService(TaskService taskService)
    {
        _taskService = taskService;
    }

    public string BuildSummary()
    {
        var tasks = _taskService.GetAll().ToList();

        // 1. Проверка на пустой список (предотвращение деления на ноль)
        if (tasks.Count == 0)
        {
            return "=== Отчёт по задачам ===\nЗадач нет";
        }

        // 2. Исправлен статус (Completed вместо "Done")
        var completed = tasks.Count(t => t.Status == TaskState.Completed);
        
        // 3. Безопасное вычисление процента
        var completionPercent = completed * 100.0 / tasks.Count;

        // 4. Проверка срока (HasValue) и исключение выполненных задач
        var overdue = tasks.Count(t => t.DueDate.HasValue && t.DueDate.Value < DateTime.Now && t.Status != TaskState.Completed);

        // 5. Исправлен приоритет (High = 2, а не 3)
        var highPriority = tasks.Count(t => t.Priority == TaskPriority.High);

        // 6. Защита от пустого исполнителя (NullReferenceException)
        var validAssignees = tasks.Where(t => !string.IsNullOrWhiteSpace(t.Assignee));
        var mostLoadedAssignee = validAssignees.Any() 
            ? validAssignees.GroupBy(t => t.Assignee!.Trim().ToLower())
                .OrderByDescending(g => g.Count())
                .First()
                .Key
            : "нет данных";

        var builder = new StringBuilder();
        builder.AppendLine("=== Отчёт по задачам ===");
        builder.AppendLine("Всего задач: " + tasks.Count);
        builder.AppendLine("Выполнено: " + completed);
        builder.AppendLine("Процент выполнения: " + completionPercent + "%");
        builder.AppendLine("Просрочено: " + overdue);
        builder.AppendLine("Высокий приоритет: " + highPriority);
        builder.AppendLine("Самый загруженный исполнитель: " + mostLoadedAssignee);
        return builder.ToString();
    }

    public List<TaskItem> GetOverdueTasks()
    {
        // Исправлено: проверка HasValue и исключение выполненных задач
        return _taskService.GetAll()
            .Where(t => t.DueDate.HasValue && t.DueDate.Value < DateTime.Now && t.Status != TaskState.Completed)
            .ToList();
    }
}