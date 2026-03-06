using MvvmCross.Plugin.Messenger;

namespace DocumentManager.Messages
{
	/// <summary>
	/// Message sent when user wants to open the Add Document window
	/// </summary>
	public class ShowAddDocumentMessage : MvxMessage
	{
		public ShowAddDocumentMessage(object sender) : base(sender)
		{
		}
	}
}
