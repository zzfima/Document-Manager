using DocumentManager.ViewModels;
using MvvmCross;
using MvvmCross.IoC;

namespace DocumentManager.Services
{
	/// <summary>
	/// ServiceLocator - manages dependency injection (creates and provides services)
	/// This is a simple implementation of the Service Locator pattern
	/// </summary>
	public static class ServiceLocator
	{
		private static bool _isInitialized;

		/// <summary>
		/// Initialize all services and register them for dependency injection
		/// </summary>
		public static void Initialize()
		{
			if (_isInitialized) return;

			// Initialize MvvmCross IoC container
			MvxIoCProvider.Initialize();
			var provider = Mvx.IoCProvider!;

			// Create service instances
			var loggingService = new LoggingService();
			var configService = new ConfigService();

			// Register services as singletons (one instance for entire app)
			provider.RegisterSingleton<ILoggingService>(loggingService);
			provider.RegisterSingleton<IConfigService>(configService);
			provider.RegisterSingleton<IHistoryService>(new HistoryService(loggingService));
			provider.RegisterSingleton<IDocumentService>(new DocumentService(configService, loggingService));
			provider.RegisterSingleton<IFileWatcherService>(new FileWatcherService(configService, loggingService));

			// Register ViewModels (new instance each time)
			provider.RegisterType<MainViewModel>();
			provider.RegisterType<AddDocumentViewModel>();

			_isInitialized = true;
		}

		/// <summary>
		/// Get a service or ViewModel from the container
		/// </summary>
		public static T Resolve<T>() where T : class
		{
			return Mvx.IoCProvider!.Resolve<T>()!;
		}
	}
}
