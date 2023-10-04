using BookClubApp.Core;
using BookClubApp.Model.Database;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace BookClubApp.ViewModel
{
    public sealed class OrdersVM : BaseVM
    {
        public RelayCommand EditOrderCommand { get; private set; }
        public RelayCommand DeleteOrderCommand { get; private set; }

        private ObservableCollection<Order> _orders;
        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set
            {
                _orders = value;
                OnPropertyChanged();
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

        public OrdersVM()
        {
            Title = "Заказы";

            EditOrderCommand = new RelayCommand(arg =>
            {
                if (arg is Order order)
                {
                    var vm = new EditOrderVM(order);
                    vm.OnEditComplete += async () => await SetOrdersAsync();
                    DialogWindow = vm;
                }
            });

            DeleteOrderCommand = new RelayCommand(async arg =>
            {
                if (arg is Order order)
                {
                    MessageBoxResult result = MessageBox.Show(
                        "Вы уверены что хотите удалить этот заказ?\nДействие нельзя будет отменить!",
                        "Подвердите удаление",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        using (BookClubEntities db = new BookClubEntities())
                        {
                            db.Order.Remove(db.Order.Find(order.ID));
                            await db.SaveChangesAsync();
                        }
                        Orders.Remove(order);
                    }
                }
            });

            Dispatcher.CurrentDispatcher.Invoke(SetOrdersAsync);
        }

        private async Task SetOrdersAsync()
        {
            BookClubEntities db = new BookClubEntities();

            try
            {
                IEnumerable<Order> orders = await db.Order.ToListAsync();
                Orders = new ObservableCollection<Order>(orders);
                await Task.Delay(1000);
            }
            finally
            {
                db.Dispose();
            }
        }
    }
}
