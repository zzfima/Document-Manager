using System.IO;

namespace DocumentManager.Services
{
	public class LoggingService : ILoggingService
	{
		private readonly string _logFilePath;
		private readonly object _lockObject = new object();

		public LoggingService()
		{
			var logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
			if (!Directory.Exists(logDirectory))
			{
				Directory.CreateDirectory(logDirectory);
			}
			_logFilePath = Path.Combine(logDirectory, $"app_{DateTime.Now:yyyy-MM-dd}.log");
		}

		public void LogInfo(string message)
		{
			WriteLog("INFO", message);
		}

		public void LogWarning(string message)
		{
			WriteLog("WARNING", message);
		}

		public void LogError(string message, Exception? exception = null)
		{
			var fullMessage = exception != null
				? $"{message} | Exception: {exception.Message}"
				: message;
			WriteLog("ERROR", fullMessage);
		}

		private void WriteLog(string level, string message)
		{
			lock (_lockObject)
			{
				try
				{
					var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
					File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
				}
				catch
				{
					// Silently fail if logging fails
				}
			}
		}
	}
}
