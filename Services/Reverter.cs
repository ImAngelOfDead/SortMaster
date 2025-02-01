using SortMasterCLI.Models;

namespace SortMasterCLI.Services;

public static class Reverter {
    public static void Revert(Config config) {
        Console.WriteLine("Запущен режим обратного перемещения файлов...");
        Logger.Log("Режим обратного перемещения файлов начат.");

        foreach (var rule in config.Rules) {
            var destinationDir = Path.Combine(config.SourceDirectory, rule.Destination);
            if (Directory.Exists(destinationDir)) {
                string[] files = Directory.GetFiles(destinationDir);
                foreach (var file in files) {
                    var fileName = Path.GetFileName(file);
                    var targetPath = Path.Combine(config.SourceDirectory, fileName);

                    if (File.Exists(targetPath)) {
                        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        var newFileName = Path.GetFileNameWithoutExtension(fileName) + "_" + timestamp +
                                          Path.GetExtension(fileName);
                        targetPath = Path.Combine(config.SourceDirectory, newFileName);
                    }

                    Console.WriteLine($"Перемещение '{file}' обратно в '{targetPath}'");
                    Logger.Log($"Перемещение '{file}' обратно в '{targetPath}'");
                    if (!config.DryRun)
                        try {
                            File.Move(file, targetPath);
                        }
                        catch (Exception ex) {
                            Console.WriteLine($"Ошибка при перемещении файла '{file}': {ex.Message}");
                            Logger.Log($"Ошибка при перемещении файла '{file}': {ex.Message}");
                        }
                }
            }
            else {
                Console.WriteLine($"Папка '{destinationDir}' не существует, пропускаем.");
                Logger.Log($"Папка '{destinationDir}' не существует, пропускаем.");
            }
        }

        Console.WriteLine("Обратное перемещение файлов завершено.");
        Logger.Log("Обратное перемещение файлов завершено.");
    }
}