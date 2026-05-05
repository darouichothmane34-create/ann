using System;
using System.IO;
using System.Threading.Tasks;

namespace AnnuaireEntreprise.Services;

/// <summary>
/// Service centralisé de logs applicatifs.
/// </summary>
public class LoggerService
{
    private readonly string _adminLogFilePath;
    private readonly string _errorLogFilePath;

    public LoggerService()
    {
        var logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        Directory.CreateDirectory(logDirectory);
        _adminLogFilePath = Path.Combine(logDirectory, "admin-access.log");
        _errorLogFilePath = Path.Combine(logDirectory, "errors.log");
    }

    public Task LogAdminAccessSuccessAsync(string message) => WriteInfoAsync(_adminLogFilePath, message);
    public Task LogAdminAccessDeniedAsync(string message) => WriteInfoAsync(_adminLogFilePath, message);
    public Task LogAdminPanelOpenedAsync(string message) => WriteInfoAsync(_adminLogFilePath, message);

    public Task LogErrorAsync(string message) => WriteErrorAsync(_errorLogFilePath, message);

    private Task WriteInfoAsync(string path, string message) => AppendAsync(path, "INFO", message);
    private Task WriteErrorAsync(string path, string message) => AppendAsync(path, "ERROR", message);

    private static Task AppendAsync(string path, string level, string message)
    {
        var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}{Environment.NewLine}";
        return File.AppendAllTextAsync(path, line);
    }
}
