using System.Windows;
using DocumentManager.Services;
using DocumentManager.ViewModels;

namespace DocumentManager.Views
{
	/// <summary>
	/// Code-behind for Add Document window
	/// Handles Drag & Drop - communication via MvvmCross Messenger
	/// </summary>
	public partial class AddDocumentWindow : Window
	{
		private readonly AddDocumentViewModel _viewModel;

		public AddDocumentWindow()
		{
			InitializeComponent();

			// Get ViewModel from ServiceLocator (dependency injection)
			_viewModel = ServiceLocator.Resolve<AddDocumentViewModel>();
			DataContext = _viewModel;
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
