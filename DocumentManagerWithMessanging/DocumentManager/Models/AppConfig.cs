namespace DocumentManager.Models
{
	public class AppConfig
	{
		public string StorageDirectory { get; set; } = "DocumentStorage";
		public List<string> SupportedExtensions { get; set; } = new List<string>
		{
			".txt", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
			".jpg", ".jpeg", ".png", ".gif", ".bmp",
			".csv", ".xml", ".json", ".html", ".htm",
			".zip", ".rar", ".7z"
		};
	}
}
