using BookClubApp.Core;
using BookClubApp.Model.Database;
using System;
using System.Threading;
using System.Windows.Threading;

namespace BookClubApp.ViewModel
{
    public sealed class MainVM : BaseVM
    {
        public event Action<Client> OnClientUpdate;

        private ProductsVM _productsVM;
        private ProductsVM ProductsVM => LazyInitializer.EnsureInitialized(ref _productsVM);

        private ExtendedProductsVM _extendedProductsVM;
        private ExtendedProductsVM ExtendedProductsVM => LazyInitializer.EnsureInitialized(ref _extendedProductsVM);

        private OrdersVM _ordersVM;
        private OrdersVM OrdersVM => LazyInitializer.EnsureInitialized(ref _ordersVM);

        public RelayCommand AuthorizeCommand { get; private set; }
        public RelayCommand UnauthorizeCommand { get; private set; }

        public RelayCommand GoToOrdersCommand { get; private set; }
        public RelayCommand GoToProductsCommand { get; private set; }
        public RelayCommand GoToExtendedProductsCommand { get; private set; }

        public MainVM()
        {
            Title = "Book Club 📚";

            CurrentView = ProductsVM;

            AuthorizeCommand = new RelayCommand(_ => DialogWindow = new AuthRegVM());

            UnauthorizeCommand = new RelayCommand(_ => GetGuest(), __ => Client != null && Client.PositionID != 1);

            GoToOrdersCommand = new RelayCommand(_ => CurrentView = OrdersVM, __ => CurrentView != OrdersVM);
            GoToProductsCommand = new RelayCommand(_ => CurrentView = ProductsVM, __ => CurrentView != ProductsVM);
            GoToExtendedProductsCommand = new RelayCommand(_ => CurrentView = ExtendedProductsVM, __ => CurrentView != ExtendedProductsVM);

            GetGuest();
        }

        private void GetGuest()
        {
            Dispatcher.CurrentDispatcher.Invoke(async () =>
            {
                using (BookClubEntities db = new BookClubEntities())
                {
                    Client = await db.Client.FindAsync(1)
                    ?? throw new NoGuestAccountException();
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

        private object _dialogWindow;
        public object DialogWindow
        {
            get => _dialogWindow;
            private set
            {
                _dialogWindow = value;
                OnPropertyChanged();
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
                OnPropertyChanged(nameof(ExtraCommandsIsVisible));
                OnPropertyChanged(nameof(IsAdmin));
                if (!ExtraCommandsIsVisible && !IsAdmin && CurrentView != ProductsVM)
                    GoToProductsCommand.Execute(null);
                OnClientUpdate?.Invoke(value);
            }
        }

        public bool ExtraCommandsIsVisible
        {
            get
            {
                if (Client != null && Client.UserPosition != null)
                {
                    return _client.UserPosition.ID != (int)UserPosition.Positions.Guest
                        && _client.UserPosition.ID != (int)UserPosition.Positions.Client;
                }
                return false;
            }
        }

        public bool IsAdmin
        {
            get
            {
                if (Client != null && Client.UserPosition != null)
                {
                    return _client.UserPosition.ID == (int)UserPosition.Positions.Administrator;
                }
                return false;
            }
        }

        public static Client GetClient() => (System.Windows.Application.Current.MainWindow.DataContext as MainVM).Client;
        public static void SetClient(Client client) => (System.Windows.Application.Current.MainWindow.DataContext as MainVM).Client = client;
    }
}
