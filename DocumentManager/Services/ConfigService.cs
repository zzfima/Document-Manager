using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace DocumentManager.Services
{
	public class ConfigService : IConfigService
	{
		private readonly string _configFilePath;
		public string StorageDirectory { get; private set; } = "DocumentStorage";
		public List<string> SupportedExtensions { get; private set; } = new List<string>();

		public ConfigService()
		{
			_configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
			LoadConfig();
		}

		private void LoadConfig()
		{
			try
			{
				if (File.Exists(_configFilePath))
				{
					var json = File.ReadAllText(_configFilePath);
					var config = JObject.Parse(json);
					StorageDirectory = config["StorageDirectory"]?.ToString() ?? "DocumentStorage";
					SupportedExtensions = config["SupportedExtensions"]?.ToObject<List<string>>() ?? new List<string>();
				}
			}
			catch
			{
				StorageDirectory = "DocumentStorage";
				SupportedExtensions = new List<string>();
			}
		}

		public string GetStorageDirectoryFullPath()
		{
			if (Path.IsPathRooted(StorageDirectory))
			{
				return StorageDirectory;
			}
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, StorageDirectory);
		}

		public bool IsExtensionSupported(string extension)
		{
			if (string.IsNullOrEmpty(extension)) return false;
			var ext = extension.StartsWith(".") ? extension : "." + extension;
			return SupportedExtensions.Any(e =>
				e.Equals(ext, StringComparison.OrdinalIgnoreCase));
		}

		public void EnsureStorageDirectoryExists()
		{
			var path = GetStorageDirectoryFullPath();
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}
	}
}
