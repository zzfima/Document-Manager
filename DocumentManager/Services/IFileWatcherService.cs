namespace DocumentManager.Services
{
	public interface IFileWatcherService : IDisposable
	{
		event EventHandler<FileChangedEventArgs>? FileChanged;
		void StartWatching();
		void StopWatching();
	}

	public class FileChangedEventArgs : EventArgs
	{
		public string FilePath { get; set; } = string.Empty;
		public FileChangeType ChangeType { get; set; }
	}

	public enum FileChangeType
	{
		Created,
		Deleted,
		Renamed
	}
}