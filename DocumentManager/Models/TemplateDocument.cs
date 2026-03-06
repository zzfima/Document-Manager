using Humanizer;

namespace DocumentManager.Models
{
    public abstract class TemplateDocument
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FileName { get; set; } = string.Empty;
        public long Size { get; set; }
        public string SizeFormatted => FormatSize(Size);

        private string FormatSize(long bytes)
        {
            string result = bytes.Bytes().Humanize();
            return result;
        }
        public string Extension { get; set; } = string.Empty;
        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}
