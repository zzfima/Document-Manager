namespace DocumentManager.Models
{
    public class HistoryEntry : TemplateDocument
    {
        public string OriginalPath { get; set; } = string.Empty;
        public string Action { get; set; } = "Added";
    }
}