using MvvmCross.Plugin.Messenger;

namespace DocumentManager.Messages
{
	/// <summary>
	/// Message sent when a document is confirmed to be added
	/// </summary>
	public class DocumentAddedMessage : MvxMessage
	{
		public string FilePath { get; }

		public DocumentAddedMessage(object sender, string filePath) : base(sender)
		{
			FilePath = filePath;
		}
	}
}
