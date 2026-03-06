namespace DocumentManager.Models
{
    public class Document : TemplateDocument
    {
        public string FullPath { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}