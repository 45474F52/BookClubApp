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
    public sealed class ExtendedProductsVM : BaseVM
    {
        public RelayCommand AddProductCommand { get; private set; }
        public RelayCommand EditProductCommand { get; private set; }
        public RelayCommand DeleteProductCommand { get; private set; }

        public ObservableCollection<Product> Products { get; private set; }

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

        public ExtendedProductsVM()
        {
            Title = "Товары 💰 (расширенная версия)";

            AddProductCommand = new RelayCommand(_ =>
            {
                var vm = new EditProductsVM();
                vm.OnEditComplete += async product =>
                {
                    using (BookClubEntities db = new BookClubEntities())
                    {
                        db.Product.Add(product);
                        await db.SaveChangesAsync();
                    }
                    Products.Add(product);
                };
                DialogWindow = vm;
            });

            EditProductCommand = new RelayCommand(arg =>
            {
                if (arg is Product product)
                {
                    var vm = new EditProductsVM(product);
                    vm.OnEditComplete += async _ => await SetProductsAsync();
                    DialogWindow = vm;
                }
            });

            DeleteProductCommand = new RelayCommand(async arg =>
            {
                if (arg is Product product)
                {
                    MessageBoxResult result = MessageBox.Show(
                        "Вы уверены что хотите удалить этот товар?\nДействие нельзя будет отменить!",
                        "Подвердите удаление",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        using (BookClubEntities db = new BookClubEntities())
                        {
                            db.Product.Remove(db.Product.Find(product.ID));
                            await db.SaveChangesAsync();
                        }
                        Products.Remove(product);
                    }
                }
            });

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
            OnPropertyChanged(nameof(Products));
        }
    }
}
