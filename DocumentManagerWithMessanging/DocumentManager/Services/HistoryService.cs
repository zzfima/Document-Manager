using DocumentManager.Models;
using Newtonsoft.Json;
using System.IO;

namespace DocumentManager.Services
{
	public class HistoryService : IHistoryService
	{
		private readonly ILoggingService _loggingService;
		private readonly string _historyJsonPath;
		private List<HistoryEntry> _history;

		public HistoryService(ILoggingService loggingService)
		{
			_loggingService = loggingService;
			_historyJsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "history.json");
			_history = new List<HistoryEntry>();
			LoadHistory();
		}

		private void LoadHistory()
		{
			try
			{
				if (File.Exists(_historyJsonPath))
				{
					var json = File.ReadAllText(_historyJsonPath);
					_history = JsonConvert.DeserializeObject<List<HistoryEntry>>(json) ?? new List<HistoryEntry>();
				}
			}
			catch (Exception ex)
			{
				_loggingService.LogError("Failed to load history from JSON", ex);
				_history = new List<HistoryEntry>();
			}
		}

		private void SaveHistory()
		{
			try
			{
				var json = JsonConvert.SerializeObject(_history, Formatting.Indented);
				File.WriteAllText(_historyJsonPath, json);
			}
			catch (Exception ex)
			{
				_loggingService.LogError("Failed to save history to JSON", ex);
			}
		}

		public List<HistoryEntry> GetHistory()
		{
			return _history.OrderByDescending(h => h.AddedAt).ToList();
		}

		public void AddHistoryEntry(string fileName, string originalPath, string extension, long size, string action = "Added")
		{
			var entry = new HistoryEntry
			{
				FileName = fileName,
				OriginalPath = originalPath,
				Extension = extension,
				Size = size,
				AddedAt = DateTime.Now,
				Action = action
			};

			_history.Add(entry);
			SaveHistory();
			_loggingService.LogInfo($"History entry added: {fileName} - {action}");
		}

		public void ClearHistory()
		{
			_history.Clear();
			SaveHistory();
			_loggingService.LogInfo("History cleared");
		}
	}
}
