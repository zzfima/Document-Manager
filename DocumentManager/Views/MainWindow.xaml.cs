using System;
using System.Windows;
using DocumentManager.ViewModels;

namespace DocumentManager.Views
{
	/// <summary>
	/// Code-behind for MainWindow
	/// Note: In MVVM, we keep minimal code here - just event handling for View-specific logic
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly MainViewModel _viewModel;

		public MainWindow(MainViewModel viewModel)
		{
			InitializeComponent();
			_viewModel = viewModel;
			DataContext = _viewModel;

			// Subscribe to ViewModel event to open Add Document window
			_viewModel.RequestAddDocumentWindow += OnRequestAddDocumentWindow;
			Closed += OnClosed;
		}

		/// <summary>
		/// Opens the Add Document window when requested by ViewModel
		/// </summary>
		private void OnRequestAddDocumentWindow(object? sender, EventArgs e)
		{
			var addWindow = new AddDocumentWindow(_viewModel);
			addWindow.Owner = this;
			addWindow.ShowDialog();
		}

		/// <summary>
		/// Cleanup when window closes
		/// </summary>
		private void OnClosed(object? sender, EventArgs e)
		{
			_viewModel.RequestAddDocumentWindow -= OnRequestAddDocumentWindow;
			_viewModel.Dispose();
		}
	}
}
