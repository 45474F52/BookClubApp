﻿using BookClubApp.Core;
using BookClubApp.Model.Database;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BookClubApp.ViewModel
{
    public sealed class ProductsVM : BaseVM
    {
        public RelayCommand OrderCommand { get; private set; }
        public RelayCommand ShowOrderCommand { get; private set; }

        public ObservableCollection<Product> Products { get; private set; }

        public bool HasOrdered => Products != null && Products.Any(p => p.ToOrder);

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

        private bool _isAdmin;
        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;
                OnPropertyChanged();
            }
        }

        public ProductsVM()
        {
            Title = "Товары 💰";

            OrderCommand = new RelayCommand(product => (product as Product)?.ChangeToOrderFlag());

            ShowOrderCommand = new RelayCommand(_ => DialogWindow = new OrderVM(Products.Where(p => p.ToOrder)));

            Dispatcher.CurrentDispatcher.Invoke(SetProductsAsync);
        }

        private async Task SetProductsAsync()
        {
            IEnumerable<Product> products = default;
            using (BookClubEntities db = new BookClubEntities())
            {
                products = await db.Product.ToListAsync();
            }
            Products = new ObservableCollection<Product>(products);
            foreach (Product product in products)
            {
                product.OnToOrderChanged += () => OnPropertyChanged(nameof(HasOrdered));
            }
            OnPropertyChanged(nameof(Products));
        }
    }
}
