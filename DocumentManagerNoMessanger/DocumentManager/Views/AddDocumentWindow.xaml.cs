using System;
using System.Windows;
using DocumentManager.Services;
using DocumentManager.ViewModels;

namespace DocumentManager.Views
{
	/// <summary>
	/// Code-behind for Add Document window
	/// Handles Drag & Drop and communicates with ViewModel
	/// </summary>
	public partial class AddDocumentWindow : Window
	{
		private readonly AddDocumentViewModel _viewModel;
		private readonly MainViewModel _mainViewModel;

		public AddDocumentWindow(MainViewModel mainViewModel)
		{
			InitializeComponent();
			_mainViewModel = mainViewModel;

			// Get ViewModel from ServiceLocator (dependency injection)
			_viewModel = ServiceLocator.Resolve<AddDocumentViewModel>();
			DataContext = _viewModel;

			// Subscribe to ViewModel events
			_viewModel.DocumentConfirmed += OnDocumentConfirmed;
			_viewModel.RequestClose += OnRequestClose;
		}

		/// <summary>
		/// When document is confirmed, add it via MainViewModel
		/// </summary>
		private void OnDocumentConfirmed(object? sender, string filePath)
		{
			_mainViewModel.AddDocument(filePath);
		}

		/// <summary>
		/// Close this window when requested
		/// </summary>
		private void OnRequestClose(object? sender, EventArgs e)
		{
			_viewModel.DocumentConfirmed -= OnDocumentConfirmed;
			_viewModel.RequestClose -= OnRequestClose;
			Close();
		}

		/// <summary>
		/// Handle file drop (Drag & Drop requirement)
		/// </summary>
		private void Window_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				var files = (string[])e.Data.GetData(DataFormats.FileDrop);
				_viewModel.HandleFileDrop(files);
			}
		}

		/// <summary>
		/// Handle drag over for visual feedback
		/// </summary>
		private void Window_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }
    }
}
