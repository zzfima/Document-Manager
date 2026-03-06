using DocumentManager.Models;
using Newtonsoft.Json;
using System.IO;

namespace DocumentManager.Services
{
	public class ConfigService : IConfigService
	{
		private readonly string _configFilePath;
		public AppConfig Config { get; private set; } = new AppConfig();

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
					Config = JsonConvert.DeserializeObject<AppConfig>(json) ?? new AppConfig();
				}
				else
				{
					Config = new AppConfig();
					SaveConfig();
				}
			}
			catch
			{
				Config = new AppConfig();
			}
		}

		private void SaveConfig()
		{
			var json = JsonConvert.SerializeObject(Config, Formatting.Indented);
			File.WriteAllText(_configFilePath, json);
		}

		public string GetStorageDirectoryFullPath()
		{
			if (Path.IsPathRooted(Config.StorageDirectory))
			{
				return Config.StorageDirectory;
			}
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Config.StorageDirectory);
		}

		public bool IsExtensionSupported(string extension)
		{
			if (string.IsNullOrEmpty(extension)) return false;
			var ext = extension.StartsWith(".") ? extension : "." + extension;
			return Config.SupportedExtensions.Any(e =>
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
