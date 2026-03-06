using MvvmCross.Plugin.Messenger;

namespace DocumentManager.Messages
{
	/// <summary>
	/// Message sent when a window should be closed
	/// </summary>
	public class CloseWindowMessage : MvxMessage
	{
		public string WindowName { get; }

		public CloseWindowMessage(object sender, string windowName) : base(sender)
		{
			WindowName = windowName;
		}
	}
}
