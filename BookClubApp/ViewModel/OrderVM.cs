using BookClubApp.Core;
using BookClubApp.Model.Database;
using System;
using System.Linq;
using System.Collections.Generic;

namespace BookClubApp.ViewModel
{
    public sealed class OrderVM : BaseVM
    {
        private readonly Random _rnd;

        public RelayCommand OrderCommand { get; private set; }

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

                    db.Order.Add(Order);
                    await db.SaveChangesAsync();
                }
            });
        }

        public OrderVM(IEnumerable<Product> products) : this()
        {
            Products = products;
            OnPropertyChanged(nameof(Products));
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
