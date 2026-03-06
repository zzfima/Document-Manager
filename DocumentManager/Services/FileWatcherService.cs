using System.IO;

namespace DocumentManager.Services
{
    public class FileWatcherService : IFileWatcherService
    {
        private readonly IConfigService _configService;
        private readonly ILoggingService _loggingService;
        private FileSystemWatcher? _watcher;
        private bool _disposed;

        public event EventHandler<FileChangedEventArgs>? FileChanged;

        public FileWatcherService(IConfigService configService, ILoggingService loggingService)
        {
            _configService = configService;
            _loggingService = loggingService;
        }

        public void StartWatching()
        {
            try
            {
                _configService.EnsureStorageDirectoryExists();
                var storagePath = _configService.GetStorageDirectoryFullPath();

                _watcher = new FileSystemWatcher(storagePath)
                {
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size,
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = false
                };

                _watcher.Created += OnFileCreated;
                _watcher.Deleted += OnFileDeleted;
                _watcher.Renamed += OnFileRenamed;

                _loggingService.LogInfo($"File watcher started for: {storagePath}");
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Failed to start file watcher", ex);
            }
        }

        public void StopWatching()
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Created -= OnFileCreated;
                _watcher.Deleted -= OnFileDeleted;
                _watcher.Renamed -= OnFileRenamed;
                _watcher.Dispose();
                _watcher = null;
                _loggingService.LogInfo("File watcher stopped");
            }
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            _loggingService.LogInfo($"External file created detected: {e.Name}");
            FileChanged?.Invoke(this, new FileChangedEventArgs
            {
                FilePath = e.FullPath,
                ChangeType = FileChangeType.Created
            });
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            _loggingService.LogInfo($"External file deletion detected: {e.Name}");
            FileChanged?.Invoke(this, new FileChangedEventArgs
            {
                FilePath = e.FullPath,
                ChangeType = FileChangeType.Deleted
            });
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            _loggingService.LogInfo($"External file rename detected: {e.OldName} -> {e.Name}");
            FileChanged?.Invoke(this, new FileChangedEventArgs
            {
                FilePath = e.FullPath,
                ChangeType = FileChangeType.Renamed
            });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    StopWatching();
                }
                _disposed = true;
            }
        }
    }
}