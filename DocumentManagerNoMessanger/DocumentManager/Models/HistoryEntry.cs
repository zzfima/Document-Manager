namespace DocumentManager.Models
{
	public class HistoryEntry
	{
		public string Id { get; set; } = Guid.NewGuid().ToString();
		public string FileName { get; set; } = string.Empty;
		public string OriginalPath { get; set; } = string.Empty;
		public string Extension { get; set; } = string.Empty;
		public long Size { get; set; }
		public DateTime AddedAt { get; set; } = DateTime.Now;
		public string Action { get; set; } = "Added";

		public string SizeFormatted => FormatSize(Size);

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
