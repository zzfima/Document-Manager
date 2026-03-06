namespace DocumentManager.Services
{
	public interface IConfigService
	{
		string StorageDirectory { get; }
		List<string> SupportedExtensions { get; }
		string GetStorageDirectoryFullPath();
		bool IsExtensionSupported(string extension);
		void EnsureStorageDirectoryExists();
	}
}