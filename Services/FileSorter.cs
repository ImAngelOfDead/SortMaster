using System.Text.RegularExpressions;
using SortMasterCLI.Models;

namespace SortMasterCLI.Services;

public class FileSorter {
    private readonly AnalyticsService _analytics;
    private readonly Config _config;
    private readonly List<SortingResult> _results = new();

    public FileSorter(Config config) {
        _config = config;
        _analytics = new AnalyticsService();
        Logger.Init(_config.LogFilePath);
    }
    
    public void Sort() {
        if (!Directory.Exists(_config.SourceDirectory)) {
            Console.WriteLine($"Исходная директория '{_config.SourceDirectory}' не существует.");
            Logger.Log($"Исходная директория '{_config.SourceDirectory}' не существует.");
            return;
        }

        Console.WriteLine(
            $"Начинаем сортировку файлов в '{_config.SourceDirectory}' (DryRun: {_config.DryRun}, Recursive: {_config.Recursive}, Interactive: {_config.Interactive})");
        Logger.Log(
            $"Начало сортировки: Source='{_config.SourceDirectory}', DryRun={_config.DryRun}, Recursive={_config.Recursive}, Interactive={_config.Interactive}");

        var searchOption = _config.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        string[] files = Directory.GetFiles(_config.SourceDirectory, "*.*", searchOption);

        foreach (var file in files) ProcessFile(file);

        Console.WriteLine("Сортировка завершена.");
        Logger.Log("Сортировка завершена.");

        var reportPath = Path.Combine(_config.SourceDirectory, "SortingReport.html");
        HTMLReportGenerator.GenerateReport(_results, reportPath, _analytics);
        Console.WriteLine($"HTML-отчет с результатами сортировки сгенерирован: {reportPath}");
        Logger.Log($"HTML-отчет с результатами сортировки сгенерирован: {reportPath}");
    }

    private void ProcessFile(string filePath) {

        if (IsInDestinationFolder(filePath))
            return;

        var fi = new FileInfo(filePath);
        var fileExtension = fi.Extension.ToLower();
        var ruleApplied = false;
        var result = new SortingResult { FilePath = filePath };


        foreach (var rule in _config.Rules) {
            if (!rule.Extensions.Contains(fileExtension))
                continue;


            if (rule.MinFileSize.HasValue && fi.Length < rule.MinFileSize.Value)
                continue;
            if (rule.MaxFileSize.HasValue && fi.Length > rule.MaxFileSize.Value)
                continue;
            if (rule.CreatedAfter.HasValue && fi.CreationTime < rule.CreatedAfter.Value)
                continue;
            if (rule.CreatedBefore.HasValue && fi.CreationTime > rule.CreatedBefore.Value)
                continue;
            if (!string.IsNullOrEmpty(rule.NamePattern))
                if (!Regex.IsMatch(fi.Name, rule.NamePattern))
                    continue;

            var destinationDir = Path.Combine(_config.SourceDirectory, rule.Destination);
            if (!Directory.Exists(destinationDir)) {
                Directory.CreateDirectory(destinationDir);
                Logger.Log($"Создана папка: {destinationDir}");
            }

            var fileName = fi.Name;
            var destinationPath = Path.Combine(destinationDir, fileName);
            destinationPath = GetUniqueFilePath(destinationPath);

            result.Action = $"Перемещение '{filePath}' в '{destinationPath}'";
            result.Destination = destinationDir;
            result.RuleApplied = rule.Destination;
            Console.WriteLine(result.Action);
            Logger.Log(result.Action);


            if (_config.Interactive) {
                Console.Write($"Переместить файл '{filePath}'? (y/n): ");
                var answer = Console.ReadLine();
                if (answer.ToLower() != "y") {
                    result.Action = "Пропущено по решению пользователя";
                    result.Success = false;
                    _results.Add(result);
                    return;
                }
            }

            if (!_config.DryRun)
                try {
                    File.Move(filePath, destinationPath);
                    result.Success = true;
                }
                catch (Exception ex) {
                    result.Success = false;
                    result.ErrorMessage = ex.Message;
                    Logger.Log($"Ошибка при перемещении файла '{filePath}': {ex.Message}");
                }
            else
                result.Success = true;

            ruleApplied = true;
            _analytics.Increment(rule.Destination);
            break;
        }

        if (!ruleApplied) {
            result.Action = "Нет подходящего правила";
            result.Destination = "-";
            result.Success = false;
            Console.WriteLine($"Для файла '{filePath}' не найдено подходящего правила.");
            Logger.Log($"Нет подходящего правила для файла '{filePath}'.");
        }

        _results.Add(result);
    }

    private bool IsInDestinationFolder(string filePath) {
        foreach (var rule in _config.Rules) {
            var destDir = Path.Combine(_config.SourceDirectory, rule.Destination);
            if (filePath.StartsWith(destDir, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private string GetUniqueFilePath(string destinationPath) {
        if (!File.Exists(destinationPath))
            return destinationPath;

        var directory = Path.GetDirectoryName(destinationPath);
        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(destinationPath);
        var extension = Path.GetExtension(destinationPath);
        var counter = 1;
        string newPath;
        do {
            newPath = Path.Combine(directory, $"{fileNameWithoutExt}_{counter}{extension}");
            counter++;
        } while (File.Exists(newPath));

        return newPath;
    }
}