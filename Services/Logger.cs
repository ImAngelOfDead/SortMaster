namespace SortMasterCLI.Services;

public static class Logger {
    private static string logFile = "SortMaster.log";

    public static void Init(string logFilePath) {
        logFile = logFilePath;
        try {
            File.WriteAllText(logFile, $"Log started: {DateTime.Now}\n");
        }
        catch {
        }
    }

    public static void Log(string message) {
        try {
            File.AppendAllText(logFile, $"{DateTime.Now}: {message}\n");
        }
        catch {
        }
    }
}