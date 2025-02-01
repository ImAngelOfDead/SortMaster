using System.Text;
using SortMasterCLI.Models;

namespace SortMasterCLI.Services;

public static class HTMLReportGenerator {
    public static void GenerateReport(List<SortingResult> results, string outputPath, AnalyticsService analytics) {
        var sb = new StringBuilder();

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang='ru'>");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset='UTF-8'>");
        sb.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        sb.AppendLine("    <title>Отчет сортировки файлов</title>");
        sb.AppendLine("    <script src='https://cdn.tailwindcss.com'></script>");
        sb.AppendLine("    <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body class='bg-gray-100'>");
        sb.AppendLine("  <div class='container mx-auto p-4'>");
        sb.AppendLine("    <h1 class='text-3xl font-bold mb-4'>Отчет сортировки файлов</h1>");
        sb.AppendLine("    <canvas id='analyticsChart' class='mb-8'></canvas>");
        sb.AppendLine("    <div class='overflow-x-auto'>");
        sb.AppendLine("      <table class='min-w-full divide-y divide-gray-200'>");
        sb.AppendLine("        <thead class='bg-gray-50'>");
        sb.AppendLine("          <tr>");
        sb.AppendLine(
            "            <th class='px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>Файл</th>");
        sb.AppendLine(
            "            <th class='px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>Действие</th>");
        sb.AppendLine(
            "            <th class='px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>Папка назначения</th>");
        sb.AppendLine(
            "            <th class='px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>Статус</th>");
        sb.AppendLine(
            "            <th class='px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>Сообщение об ошибке</th>");
        sb.AppendLine("          </tr>");
        sb.AppendLine("        </thead>");
        sb.AppendLine("        <tbody class='bg-white divide-y divide-gray-200'>");

        foreach (var result in results) {
            sb.AppendLine("          <tr>");
            sb.AppendLine(
                $"            <td class='px-6 py-4 whitespace-nowrap text-sm text-gray-900'>{result.FilePath}</td>");
            sb.AppendLine(
                $"            <td class='px-6 py-4 whitespace-nowrap text-sm text-gray-900'>{result.Action}</td>");
            sb.AppendLine(
                $"            <td class='px-6 py-4 whitespace-nowrap text-sm text-gray-900'>{result.Destination}</td>");

            if (result.Success)
                sb.AppendLine(
                    "            <td class='px-6 py-4 whitespace-nowrap text-sm text-green-600 font-bold'>Успешно</td>");
            else
                sb.AppendLine(
                    "            <td class='px-6 py-4 whitespace-nowrap text-sm text-red-600 font-bold'>Ошибка</td>");

            sb.AppendLine(
                $"            <td class='px-6 py-4 whitespace-nowrap text-sm text-gray-900'>{result.ErrorMessage}</td>");
            sb.AppendLine("          </tr>");
        }

        sb.AppendLine("        </tbody>");
        sb.AppendLine("      </table>");
        sb.AppendLine("    </div>");
        sb.AppendLine("  </div>");

        // Генерация скрипта для Chart.js (столбчатая диаграмма)
        sb.AppendLine("<script>");
        sb.AppendLine("  const ctx = document.getElementById('analyticsChart').getContext('2d');");
        sb.AppendLine("  const data = {");
        sb.AppendLine("    labels: [");

        var first = true;
        foreach (var label in analytics.GetData().Keys) {
            if (!first) sb.Append(", ");
            sb.Append($"'{label}'");
            first = false;
        }

        sb.AppendLine("],");
        sb.AppendLine("    datasets: [{");
        sb.AppendLine("      label: 'Файлы по категориям',");
        sb.AppendLine("      data: [");

        first = true;
        foreach (var count in analytics.GetData().Values) {
            if (!first) sb.Append(", ");
            sb.Append($"{count}");
            first = false;
        }

        sb.AppendLine("],");
        sb.AppendLine("      backgroundColor: 'rgba(75, 192, 192, 0.2)',");
        sb.AppendLine("      borderColor: 'rgba(75, 192, 192, 1)',");
        sb.AppendLine("      borderWidth: 1");
        sb.AppendLine("    }]");
        sb.AppendLine("  };");
        sb.AppendLine(
            "  const config = { type: 'bar', data: data, options: { scales: { y: { beginAtZero: true } } } };");
        sb.AppendLine("  new Chart(ctx, config);");
        sb.AppendLine("</script>");

        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        File.WriteAllText(outputPath, sb.ToString());
    }
}