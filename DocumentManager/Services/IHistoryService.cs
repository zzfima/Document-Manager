using DocumentManager.Models;

namespace DocumentManager.Services
{
    public interface IHistoryService
    {
        List<HistoryEntry> GetHistory();
        void AddHistoryEntry(string fileName, string originalPath, string extension, long size, string action = "Added");
        void ClearHistory();
    }
}
