using System.IO;
using DocumentManager.Services;
using DocumentManager.Messages;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;

namespace DocumentManager.ViewModels
{
	/// <summary>
	/// ViewModel for the Add Document window
	/// </summary>
	public class AddDocumentViewModel : ViewModelBase
	{
		// Services
		private readonly IConfigService _configService;
		private readonly ILoggingService _loggingService;
		private readonly IMvxMessenger _messenger;

		private string _selectedFilePath = string.Empty;
		private string _fileName = string.Empty;
		private string _fileType = string.Empty;
		private string _fileSize = string.Empty;
		private string _filePath = string.Empty;
		private string _createdAt = string.Empty;
		private string _modifiedAt = string.Empty;
		private bool _isFileSelected;
		private bool _isValidFile;
		private string _errorMessage = string.Empty;

		public string SelectedFilePath
		{
			get => _selectedFilePath;
			set
			{
				if (SetProperty(ref _selectedFilePath, value))
				{
					UpdateFileInfo();
				}
			}
		}

		public string FileName
		{
			get => _fileName;
			set => SetProperty(ref _fileName, value);
		}

		public string FileType
		{
			get => _fileType;
			set => SetProperty(ref _fileType, value);
		}

		public string FileSize
		{
			get => _fileSize;
			set => SetProperty(ref _fileSize, value);
		}

		public string FilePath
		{
			get => _filePath;
			set => SetProperty(ref _filePath, value);
		}

		public string CreatedAt
		{
			get => _createdAt;
			set => SetProperty(ref _createdAt, value);
		}

		public string ModifiedAt
		{
			get => _modifiedAt;
			set => SetProperty(ref _modifiedAt, value);
		}

		public bool IsFileSelected
		{
			get => _isFileSelected;
			set => SetProperty(ref _isFileSelected, value);
		}

		public bool IsValidFile
		{
			get => _isValidFile;
			set
			{
				if (SetProperty(ref _isValidFile, value))
				{
					ConfirmCommand.RaiseCanExecuteChanged();
				}
			}
		}

		public string ErrorMessage
		{
			get => _errorMessage;
			set => SetProperty(ref _errorMessage, value);
		}

		// Commands
		public IMvxCommand BrowseCommand { get; }
		public MvxCommand ConfirmCommand { get; }
		public IMvxCommand CancelCommand { get; }

		/// <summary>
		/// Constructor - receives services via dependency injection
		/// </summary>
		public AddDocumentViewModel(IConfigService configService, ILoggingService loggingService, IMvxMessenger messenger)
		{
			_configService = configService;
			_loggingService = loggingService;
			_messenger = messenger;

			BrowseCommand = new MvxCommand(BrowseFile);
			ConfirmCommand = new MvxCommand(Confirm, () => IsValidFile);
			CancelCommand = new MvxCommand(Cancel);
		}

        private void BrowseFile()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select a document",
                Filter = "All supported files|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                SelectedFilePath = dialog.FileName;
                _loggingService.LogInfo($"File selected via browse: {dialog.FileName}");
            }
        }

        public void HandleFileDrop(string[] files)
        {
            if (files != null && files.Length > 0)
            {
                SelectedFilePath = files[0];
                _loggingService.LogInfo($"File dropped: {files[0]}");
            }
        }

        private void UpdateFileInfo()
        {
            ErrorMessage = string.Empty;
            IsFileSelected = false;
            IsValidFile = false;

            if (string.IsNullOrEmpty(SelectedFilePath) || !File.Exists(SelectedFilePath))
            {
                ClearFileInfo();
                return;
            }

            try
            {
                var fileInfo = new FileInfo(SelectedFilePath);
                var extension = fileInfo.Extension;

                FileName = fileInfo.Name;
                FileType = string.IsNullOrEmpty(extension) ? "Unknown" : extension.ToUpperInvariant().TrimStart('.');
                FileSize = FormatSize(fileInfo.Length);
                FilePath = fileInfo.FullName;
                CreatedAt = fileInfo.CreationTime.ToString("yyyy-MM-dd HH:mm:ss");
                ModifiedAt = fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");

                IsFileSelected = true;

                if (!_configService.IsExtensionSupported(extension))
                {
                    ErrorMessage = $"Unsupported file type: {extension}. Please select a supported file type.";
                    IsValidFile = false;
                    _loggingService.LogWarning($"Unsupported file type: {extension}");
                }
                else
                {
                    IsValidFile = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error reading file: {ex.Message}";
                _loggingService.LogError("Error reading file info", ex);
                ClearFileInfo();
            }
        }

        private void ClearFileInfo()
        {
            FileName = string.Empty;
            FileType = string.Empty;
            FileSize = string.Empty;
            FilePath = string.Empty;
            CreatedAt = string.Empty;
            ModifiedAt = string.Empty;
        }

		/// <summary>
		/// Confirms the document selection - publishes messages via Messenger
		/// </summary>
		private void Confirm()
		{
			if (IsValidFile && !string.IsNullOrEmpty(SelectedFilePath))
			{
				// Publish message to add document
				_messenger.Publish(new DocumentAddedMessage(this, SelectedFilePath));
				// Publish message to close window
				_messenger.Publish(new CloseWindowMessage(this, "AddDocument"));
			}
		}

		/// <summary>
		/// Cancels and closes the window - publishes message via Messenger
		/// </summary>
		private void Cancel()
		{
			_messenger.Publish(new CloseWindowMessage(this, "AddDocument"));
		}

        private static string FormatSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }
            return $"{size:0.##} {sizes[order]}";
        }
    }
}
