using Newtonsoft.Json;
using SortMasterCLI.Models;
using SortMasterCLI.Services;
using System.Globalization;

namespace SortMasterCLI;

internal class Program {
    private static async Task Main(string[] args) {
        var revertMode = args.Any(arg => arg.Equals("-back", StringComparison.OrdinalIgnoreCase));
        
        if (args.Any(arg => arg.Equals("-help", StringComparison.OrdinalIgnoreCase) || 
                            arg.Equals("--help", StringComparison.OrdinalIgnoreCase))) {
            PrintHelp();
            return;
        }
        
        var configPath = args.FirstOrDefault(arg => !arg.StartsWith("-")) ?? "config.json";
        var username = Other.Utils.GetUserName();
        if (!File.Exists(configPath)) {

            var defaultConfig = new Config {
                SourceDirectory = $"{username}\\Downloads",
                DryRun = true,
                Recursive = false,
                Interactive = false,
                LogFilePath = "SortMaster.log",
                ProfileName = "default",
                Rules = new List<Rule> {
                    new Rule {
                        Extensions = new List<string> { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".avif", ".avifs" },
                        Destination = "Images"
                    },
                    new Rule {
                        Extensions = new List<string> { ".mp4", ".avi", ".mkv", ".mov", ".wmv" },
                        Destination = "Videos"
                    },
                    new Rule {
                        Extensions = new List<string> { ".exe", ".msi" },
                        Destination = "Installers"
                    },
                    new Rule {
                        Extensions = new List<string> { ".zip", ".rar", ".7z", ".tar", ".gz" },
                        Destination = "Archives"
                    },
                    new Rule {
                        Extensions = new List<string> { ".txt", ".doc", ".docx", ".pdf", ".xls", ".xlsx", ".ppt", ".pptx", ".json", ".html", ".htm" },
                        Destination = "Documents"
                    }
                }
            };

  
            var defaultConfigJson = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);

            File.WriteAllText(configPath, defaultConfigJson);
            Console.WriteLine($"Базовый конфигурационный файл '{configPath}' был автоматически сгенерирован.");
            Console.WriteLine("Отредактируйте его при необходимости и запустите программу снова.");
            return;
        }
        
        Config config;
        try {
            var configJson = File.ReadAllText(configPath);
            config = JsonConvert.DeserializeObject<Config>(configJson);
        }
        catch (Exception ex) {
            Console.WriteLine($"Ошибка при чтении файла конфигурации: {ex.Message}");
            File.Delete(configPath);
            await Main(args);

            return;
        }

        if (config == null) {
            Console.WriteLine("Конфигурация пуста или имеет неверный формат.");
            File.Delete(configPath);
            await Main(args);

            return;
        }

        if (revertMode) {
            Reverter.Revert(config);
        }
        else {
            var sorter = new FileSorter(config);
            sorter.Sort();
        }

        void PrintHelp() {
            Console.WriteLine("SortMaster - Утилита для сортировки файлов по заданным правилам.");
            Console.WriteLine();
            Console.WriteLine("Использование:");
            Console.WriteLine("  SortMaster.exe [путь_к_конфигу] [опции]");
            Console.WriteLine();
            Console.WriteLine("Опции:");
            Console.WriteLine("  -help, --help       Показать эту справку и выйти.");
            Console.WriteLine("  -back               Запустить режим обратного перемещения файлов (откат изменений).");
            Console.WriteLine("  (Если аргументы не указаны, будет использоваться 'config.json' в текущей папке.)");
            Console.WriteLine();
            Console.WriteLine("Пример:");
            Console.WriteLine("  SortMaster.exe config.json -back");
        }
        
    }
}
