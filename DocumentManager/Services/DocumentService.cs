using DocumentManager.Models;
using Newtonsoft.Json;
using System.IO;

namespace DocumentManager.Services
{
	public class DocumentService : IDocumentService
	{
		private readonly IConfigService _configService;
		private readonly ILoggingService _loggingService;
		private readonly string _documentsJsonPath;
		private List<Document> _documents;

		public DocumentService(IConfigService configService, ILoggingService loggingService)
		{
			_configService = configService;
			_loggingService = loggingService;
			_documentsJsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "documents.json");
			_documents = new List<Document>();
			LoadDocuments();
		}

		private void LoadDocuments()
		{
			try
			{
				if (File.Exists(_documentsJsonPath))
				{
					var json = File.ReadAllText(_documentsJsonPath);
					_documents = JsonConvert.DeserializeObject<List<Document>>(json) ?? new List<Document>();
				}
			}
			catch (Exception ex)
			{
				_loggingService.LogError("Failed to load documents from JSON", ex);
				_documents = new List<Document>();
			}
		}

		public List<Document> GetAllDocuments()
		{
			return _documents.ToList();
		}

		public Document? AddDocument(string sourceFilePath)
		{
			try
			{
				if (!File.Exists(sourceFilePath))
				{
					_loggingService.LogError($"Source file not found: {sourceFilePath}");
					return null;
				}

				var extension = Path.GetExtension(sourceFilePath);
				if (!_configService.IsExtensionSupported(extension))
				{
					_loggingService.LogWarning($"Unsupported file type: {extension}");
					return null;
				}

				_configService.EnsureStorageDirectoryExists();
				var storageDir = _configService.GetStorageDirectoryFullPath();
				var fileName = Path.GetFileName(sourceFilePath);
				var destPath = Path.Combine(storageDir, fileName);

				// Handle duplicate file names
				var counter = 1;
				var nameWithoutExt = Path.GetFileNameWithoutExtension(sourceFilePath);
				while (File.Exists(destPath))
				{
					fileName = $"{nameWithoutExt}_{counter}{extension}";
					destPath = Path.Combine(storageDir, fileName);
					counter++;
				}

				File.Copy(sourceFilePath, destPath);

				var fileInfo = new FileInfo(destPath);
				var document = new Document
				{
					Name = fileName,
					FullPath = destPath,
					Extension = extension,
					Size = fileInfo.Length,
					CreatedAt = fileInfo.CreationTime,
					ModifiedAt = fileInfo.LastWriteTime,
					AddedAt = DateTime.Now
				};

				_documents.Add(document);
				SaveDocuments(_documents);

				_loggingService.LogInfo($"Document added: {fileName}");
				return document;
			}
			catch (Exception ex)
			{
				_loggingService.LogError($"Failed to add document: {sourceFilePath}", ex);
				return null;
			}
		}

		public void RemoveDocument(string documentId)
		{
			var document = _documents.FirstOrDefault(d => d.Id == documentId);
			if (document != null)
			{
				_documents.Remove(document);
				SaveDocuments(_documents);
				_loggingService.LogInfo($"Document removed: {document.Name}");
			}
		}

		public void SaveDocuments(List<Document> documents)
		{
			try
			{
				_documents = documents;
				var json = JsonConvert.SerializeObject(_documents, Formatting.Indented);
				File.WriteAllText(_documentsJsonPath, json);
			}
			catch (Exception ex)
			{
				_loggingService.LogError("Failed to save documents to JSON", ex);
			}
		}

		public Document? CreateDocumentFromFile(string filePath)
		{
			try
			{
				if (!File.Exists(filePath)) return null;

				var fileInfo = new FileInfo(filePath);
				return new Document
				{
					Name = fileInfo.Name,
					FullPath = filePath,
					Extension = fileInfo.Extension,
					Size = fileInfo.Length,
					CreatedAt = fileInfo.CreationTime,
					ModifiedAt = fileInfo.LastWriteTime,
					AddedAt = DateTime.Now
				};
			}
			catch
			{
				return null;
			}
		}

		public void RefreshDocumentsFromStorage()
		{
			try
			{
				_configService.EnsureStorageDirectoryExists();
				var storageDir = _configService.GetStorageDirectoryFullPath();
				var filesInStorage = Directory.GetFiles(storageDir);

				// Remove documents that no longer exist in storage
				_documents.RemoveAll(d => !File.Exists(d.FullPath));

				// Add new files that are in storage but not in documents list
				foreach (var filePath in filesInStorage)
				{
					if (!_documents.Any(d => d.FullPath.Equals(filePath, StringComparison.OrdinalIgnoreCase)))
					{
						var doc = CreateDocumentFromFile(filePath);
						if (doc != null && _configService.IsExtensionSupported(doc.Extension))
						{
							_documents.Add(doc);
							_loggingService.LogInfo($"External file detected and added: {doc.Name}");
						}
					}
				}

				SaveDocuments(_documents);
			}
			catch (Exception ex)
			{
				_loggingService.LogError("Failed to refresh documents from storage", ex);
			}
		}
	}
}
