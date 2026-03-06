using System.Windows;
using DocumentManager.Services;
using DocumentManager.ViewModels;
using DocumentManager.Views;

namespace DocumentManager
{
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			// Initialize MvvmCross IoC container and services
			ServiceLocator.Initialize();

			// Resolve MainViewModel with automatic constructor injection
			var mainViewModel = ServiceLocator.Resolve<MainViewModel>();

			var mainWindow = new MainWindow(mainViewModel);
			mainWindow.Show();
		}
	}
}
