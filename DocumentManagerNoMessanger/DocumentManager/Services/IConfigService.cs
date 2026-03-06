using DocumentManager.Models;

namespace DocumentManager.Services
{
	public interface IConfigService
	{
		AppConfig Config { get; }
		string GetStorageDirectoryFullPath();
		bool IsExtensionSupported(string extension);
		void EnsureStorageDirectoryExists();
	}
}
