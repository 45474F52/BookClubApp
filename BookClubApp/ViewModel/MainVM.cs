using BookClubApp.Core;
using BookClubApp.Model.Database;
using System;
using System.Threading;
using System.Windows.Threading;

namespace BookClubApp.ViewModel
{
    public sealed class MainVM : BaseVM
    {
		private ProductsVM _productsVM;
		private ProductsVM ProductsVM => LazyInitializer.EnsureInitialized(ref _productsVM);

        public RelayCommand AuthorizeCommand { get; private set; }
        public RelayCommand UnauthorizeCommand { get; private set; }

        public MainVM()
        {
            Title = "Book Club 📚";

			CurrentView = ProductsVM;

            AuthorizeCommand = new RelayCommand(_ => { }, __ => false);
            UnauthorizeCommand = new RelayCommand(_ => { }, __ => false);

			Dispatcher.CurrentDispatcher.Invoke(async () =>
			{
				using (BookClubEntities db = new BookClubEntities())
				{
					Client = await db.Client.FindAsync(1)
					?? throw new InvalidOperationException("Отсутствует гостевой аккаунт");
				}
			});
        }

		private BaseVM _currentView;
		public BaseVM CurrentView
		{
			get => _currentView;
			private set
			{
				if (_currentView != value)
				{
                    _currentView = value;
					Title = value.Title;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Title));
                }
			}
		}

		private Client _client;
		public Client Client
		{
			get => _client;
			private set
			{
				_client = value;
				OnPropertyChanged();
			}
		}

        public static Client GetClient() => (System.Windows.Application.Current.MainWindow.DataContext as MainVM).Client;
    }
}
