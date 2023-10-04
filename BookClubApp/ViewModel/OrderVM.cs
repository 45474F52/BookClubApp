using BookClubApp.Core;
using BookClubApp.Model.Database;
using System;
using System.Linq;
using System.Data.Entity;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Windows;

namespace BookClubApp.ViewModel
{
    public sealed class OrderVM : BaseVM
    {
        private readonly Random _rnd;

        public RelayCommand OrderCommand { get; private set; }

        public RelayCommand IncreaseCommand { get; private set; }
        public RelayCommand DecreaseCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }

        public Order Order { get; } = new Order();

        public IEnumerable<Product> Products { get; private set; }

        public IEnumerable<OrderPickupPoint> OrderPickupPoints { get; private set; }

        private OrderPickupPoint _selectedPoint;
        public OrderPickupPoint SelectedPoint
        {
            get => _selectedPoint;
            set
            {
                _selectedPoint = value;
                OnPropertyChanged();
            }
        }

        public decimal OrderSumm => Order.Product.Sum(p => p.TotalPrice);

        public OrderVM()
        {
            _rnd = new Random();

            Title = "Заказ";

            OrderCommand = new RelayCommand(async _ =>
            {
                using (BookClubEntities db = new BookClubEntities())
                {
                    Client client = (MainVM.GetClient() ?? await db.Client.FindAsync(1))
                    ?? throw new NoGuestAccountException("Невозможно создать заказ без аккаунта");

                    Order.ClientID = client.ID;
                    Order.StatusID = 1;
                    Order.CreationDate = DateTime.Now;
                    Order.PickupPointID = SelectedPoint.ID;
                    Order.DeliveryTimeInDays = GetDeliveryTime();
                    Order.PickupCode = GetPickupCode(db);
                    Order.Summ = OrderSumm;

                    db.Order.Add(Order); // !!! Влияет на состояние товаров
                    foreach (Product product in Order.Product)
                        db.Entry(product).State = EntityState.Unchanged;

                    await db.SaveChangesAsync();
                    MessageBox.Show(
                        string.Format(
                            "Заказ успешно создан!\n\n" +
                            "Номер талона: {0}\n\n" +
                            "Срок выполнения заказа (дней): {1}",
                            Order.PickupCode, Order.DeliveryTimeInDays),
                        "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }, __ => Order.Product.Any() && SelectedPoint != null);

            IncreaseCommand = new RelayCommand(arg =>
            {
                if (arg is Product product)
                {
                    Product target = Order.Product.First(p => p.ID == product.ID);
                    ++target.CountInOrder;
                    OnPropertyChanged(nameof(Order));
                    OnPropertyChanged(nameof(OrderSumm));
                }
            });

            DecreaseCommand = new RelayCommand(arg =>
            {
                if (arg is Product product)
                {
                    Product target = Order.Product.First(p => p.ID == product.ID);
                    --target.CountInOrder;
                    if (target.CountInOrder == 0)
                        RemoveCommand.Execute(product);
                    else
                    {
                        OnPropertyChanged(nameof(Order));
                        OnPropertyChanged(nameof(OrderSumm));
                    }
                }
            });

            RemoveCommand = new RelayCommand(arg =>
            {
                if (arg is Product product)
                {
                    Product target = Order.Product.First(p => p.ID == product.ID);
                    target.ChangeToOrderFlag();
                    Products = Products.Where(p => p.ID != target.ID);
                    OnPropertyChanged(nameof(Products));
                    Order.Product.Remove(target);
                    OnPropertyChanged(nameof(Order));
                    OnPropertyChanged(nameof(OrderSumm));
                }
            });

            Dispatcher.CurrentDispatcher.Invoke(async () =>
            {
                using (BookClubEntities db = new BookClubEntities())
                {
                    OrderPickupPoints = await db.OrderPickupPoint.ToListAsync();
                    OnPropertyChanged(nameof(OrderPickupPoints));
                }
            });
        }

        public OrderVM(IEnumerable<Product> products) : this()
        {
            Products = products;
            OnPropertyChanged(nameof(Products));
            Order.Product = products.ToList();
        }

        private string GetDeliveryTime() => Products.Any(p => p.Quantity < 3) ? "6" : "3";

        private string GetPickupCode(BookClubEntities db)
        {
            int num1, num2, num3;
            string result = string.Empty;
            do
            {
                num1 = _rnd.Next(0, 10);
                num2 = _rnd.Next(0, 10);
                num3 = _rnd.Next(0, 10);
                result = string.Format("{0}{1}{2}", num1, num2, num3);
            }
            while (db.Order.AsNoTracking().Any(o => o.PickupCode == result));

            return result;
        }
    }
}
