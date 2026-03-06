using System.Collections.ObjectModel;
using System.Windows;
using DocumentManager.Models;
using DocumentManager.Services;
using MvvmCross.Commands;

namespace DocumentManager.ViewModels
{
	/// <summary>
	/// ViewModel for the main window - handles Documents list and History list
	/// </summary>
	public class MainViewModel : ViewModelBase, IDisposable
	{
		// Services (injected via constructor)
		private readonly IDocumentService _documentService;
		private readonly IHistoryService _historyService;
		private readonly ILoggingService _loggingService;
		private readonly IFileWatcherService _fileWatcherService;
		private bool _disposed;

		// Observable collections for data binding
		private ObservableCollection<Document> _documents = new();
		private ObservableCollection<HistoryEntry> _history = new();

		// Documents collection - bound to ListView in Tab 1
		public ObservableCollection<Document> Documents
		{
			get => _documents;
			set => SetProperty(ref _documents, value);
		}

		// History collection - bound to ListView in Tab 2
		public ObservableCollection<HistoryEntry> History
		{
			get => _history;
			set => SetProperty(ref _history, value);
		}

		// Commands for buttons
		public IMvxCommand AddDocumentCommand { get; }
		public IMvxCommand ClearHistoryCommand { get; }

		// Event to request opening Add Document window
		public event EventHandler? RequestAddDocumentWindow;

		/// <summary>
		/// Constructor - receives services via dependency injection
		/// </summary>
		public MainViewModel(
			IDocumentService documentService,
			IHistoryService historyService,
			ILoggingService loggingService,
			IFileWatcherService fileWatcherService)
		{
			_documentService = documentService;
			_historyService = historyService;
			_loggingService = loggingService;
			_fileWatcherService = fileWatcherService;

			// Create commands (MVVM pattern - no code in code-behind)
			AddDocumentCommand = new MvxCommand(OpenAddDocumentWindow);
			ClearHistoryCommand = new MvxCommand(ClearHistory);

			// Start file system monitoring (requirement 7)
			_fileWatcherService.FileChanged += OnFileChanged;
			_fileWatcherService.StartWatching();

			// Load data from JSON on startup (requirement 5)
			LoadData();
			_loggingService.LogInfo("Application started");
		}

		/// <summary>
		/// Loads documents and history from JSON files
		/// </summary>
		private void LoadData()
		{
			try
			{
				_documentService.RefreshDocumentsFromStorage();
				var documents = _documentService.GetAllDocuments();
				Documents = new ObservableCollection<Document>(documents);

				var history = _historyService.GetHistory();
				History = new ObservableCollection<HistoryEntry>(history);

				_loggingService.LogInfo("Documents loaded");
			}
			catch (Exception ex)
			{
				_loggingService.LogError("Failed to load data", ex);
			}
		}

		/// <summary>
		/// Opens the Add Document window
		/// </summary>
		private void OpenAddDocumentWindow()
		{
			_loggingService.LogInfo("Add window opened");
			RequestAddDocumentWindow?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Adds a document to the system (called from AddDocumentWindow)
		/// </summary>
		public void AddDocument(string filePath)
		{
			try
			{
				var document = _documentService.AddDocument(filePath);
				if (document != null)
				{
					Documents.Add(document);
					
					// Add to history
					_historyService.AddHistoryEntry(
						document.Name,
						filePath,
						document.Extension,
						document.Size,
						"Added");
					RefreshHistory();
					
					_loggingService.LogInfo($"Document added: {document.Name}");
				}
			}
			catch (Exception ex)
			{
				_loggingService.LogError("Failed to add document", ex);
				MessageBox.Show($"Failed to add document: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary>
		/// Clears the history (requirement: Clear History button)
		/// </summary>
		private void ClearHistory()
		{
			try
			{
				_historyService.ClearHistory();
				History.Clear();
				_loggingService.LogInfo("Clear history");
			}
			catch (Exception ex)
			{
				_loggingService.LogError("Failed to clear history", ex);
			}
		}

		/// <summary>
		/// Refreshes history from JSON
		/// </summary>
		private void RefreshHistory()
		{
			var history = _historyService.GetHistory();
			History = new ObservableCollection<HistoryEntry>(history);
		}

		/// <summary>
		/// Handles file system changes (requirement 7 - real-time monitoring)
		/// Uses Dispatcher for thread safety
		/// </summary>
		private void OnFileChanged(object? sender, FileChangedEventArgs e)
		{
			// Thread-safe UI update using Dispatcher
			Application.Current.Dispatcher.Invoke(() =>
			{
				try
				{
					_documentService.RefreshDocumentsFromStorage();
					var documents = _documentService.GetAllDocuments();
					Documents = new ObservableCollection<Document>(documents);

					// Log external changes
					if (e.ChangeType == FileChangeType.Created)
					{
						var fileName = System.IO.Path.GetFileName(e.FilePath);
						var extension = System.IO.Path.GetExtension(e.FilePath);
						long size = 0;
						if (System.IO.File.Exists(e.FilePath))
						{
							size = new System.IO.FileInfo(e.FilePath).Length;
						}
						_historyService.AddHistoryEntry(fileName, e.FilePath, extension, size, "External Add");
						RefreshHistory();
					}
					else if (e.ChangeType == FileChangeType.Deleted)
					{
						var fileName = System.IO.Path.GetFileName(e.FilePath);
						_historyService.AddHistoryEntry(fileName, e.FilePath, "", 0, "External Delete");
						RefreshHistory();
					}

					_loggingService.LogInfo($"External file change detected: {e.ChangeType}");
				}
				catch (Exception ex)
				{
					_loggingService.LogError("Failed to handle file change", ex);
				}
			});
		}

		/// <summary>
		/// Cleanup resources
		/// </summary>
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
					_fileWatcherService.FileChanged -= OnFileChanged;
					_fileWatcherService.Dispose();
				}
				_disposed = true;
			}
		}
	}
}
