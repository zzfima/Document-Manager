using DocumentManager.Models;

namespace DocumentManager.Services
{
	public interface IDocumentService
	{
		List<Document> GetAllDocuments();
		Document? AddDocument(string sourceFilePath);
		void SaveDocuments(List<Document> documents);
		Document? CreateDocumentFromFile(string filePath);
		void RefreshDocumentsFromStorage();
	}
}