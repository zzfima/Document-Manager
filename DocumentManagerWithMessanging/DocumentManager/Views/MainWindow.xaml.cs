using System;
using System.Windows;
using DocumentManager.Services;
using DocumentManager.ViewModels;
using DocumentManager.Messages;
using MvvmCross.Plugin.Messenger;

namespace DocumentManager.Views
{
	/// <summary>
	/// Code-behind for MainWindow
	/// Subscribes to messages via MvvmCross Messenger
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly MainViewModel _viewModel;
		private readonly IMvxMessenger _messenger;
		private readonly MvxSubscriptionToken _showAddDocumentToken;
		private readonly MvxSubscriptionToken _closeWindowToken;

		public MainWindow(MainViewModel viewModel)
		{
			InitializeComponent();
			_viewModel = viewModel;
			DataContext = _viewModel;

			// Get messenger from ServiceLocator
			_messenger = ServiceLocator.Resolve<IMvxMessenger>();

			// Subscribe to messages
			_showAddDocumentToken = _messenger.Subscribe<ShowAddDocumentMessage>(OnShowAddDocument);
			_closeWindowToken = _messenger.Subscribe<CloseWindowMessage>(OnCloseWindow);

			Closed += OnClosed;
		}

		/// <summary>
		/// Opens the Add Document window when message received
		/// </summary>
		private void OnShowAddDocument(ShowAddDocumentMessage message)
		{
			var addWindow = new AddDocumentWindow();
			addWindow.Owner = this;
			addWindow.ShowDialog();
		}

		/// <summary>
		/// Closes window when message received
		/// </summary>
		private void OnCloseWindow(CloseWindowMessage message)
		{
			if (message.WindowName == "AddDocument")
			{
				// Find and close AddDocumentWindow
				foreach (Window window in Application.Current.Windows)
				{
					if (window is AddDocumentWindow)
					{
						window.Close();
						break;
					}
				}
			}
		}

		/// <summary>
		/// Cleanup when window closes
		/// </summary>
		private void OnClosed(object? sender, EventArgs e)
		{
			// Unsubscribe from messages
			_messenger.Unsubscribe<ShowAddDocumentMessage>(_showAddDocumentToken);
			_messenger.Unsubscribe<CloseWindowMessage>(_closeWindowToken);
			_viewModel.Dispose();
		}
	}
}
