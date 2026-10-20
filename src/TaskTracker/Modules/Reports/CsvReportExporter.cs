using System.Text;
using TaskTracker.Models;

namespace TaskTracker.Modules.Reports;

public class CsvReportExporter
{
    public void Export(IEnumerable<TaskItem> tasks)
    {
        var path = @"C:\TaskTracker\reports\tasks.csv"; // Можно заменить на "reports.csv"

        try
        {
            var lines = new List<string>
            {
                "Id,Title,Status,Priority,Assignee,DueDate"
            };

            foreach (var task in tasks)
            {
                // ИСПРАВЛЕНИЕ: Оборачиваем текстовые поля в кавычки и экранируем внутренние кавычки
                var title = $"\"{task.Title?.Replace("\"", "\"\"")}\"";
                var assignee = $"\"{task.Assignee?.Replace("\"", "\"\"")}\"";

                lines.Add(string.Join(",", 
                    task.Id, 
                    title, 
                    task.Status, 
                    task.Priority, 
                    assignee, 
                    task.DueDate));
            }

            File.WriteAllLines(path, lines, Encoding.UTF8);
            Console.WriteLine("Отчёт сохранён: " + path);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка экспорта: " + ex.Message);
        }
    }
}