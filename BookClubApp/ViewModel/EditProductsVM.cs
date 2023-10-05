using BookClubApp.Core;
using BookClubApp.Model.Database;
using System;
using System.Windows.Threading;

namespace BookClubApp.ViewModel
{
    public sealed class EditProductsVM : BaseVM
    {
        public event Action<Product> OnEditComplete = delegate { };

        public EditProductsVM()
        {
            Title = $"Редактор продукта ✏";

            ApplyCommand = new RelayCommand(async _ =>
            {
                if (HasDirty)
                {
                    using (BookClubEntities db = new BookClubEntities())
                    {
                        if (Product != null)
                        {
                            Product target = db.Product.Find(Product.ID);

                            if (_nameIsDirty)
                                target.Name = Name;
                            if (_descriptionIsDirty)
                                target.Description = Description;
                            if (_pathIsDirty)
                                target.PathToImage = PathToImage;
                            if (_priceIsDirty)
                                target.Price = Price;
                            if (_percentIsDirty)
                                target.DiscountPercent = DiscountPercent;
                            if (_quantityIsDirty)
                                target.Quantity = Quantity;
                        }
                        else
                        {
                            Product = new Product()
                            {
                                Name = Name,
                                Description = Description,
                                PathToImage = PathToImage,
                                Price = Price,
                                DiscountPercent = DiscountPercent,
                                Quantity = Quantity
                            };

                            db.Product.Add(Product);
                        }

                        await db.SaveChangesAsync();
                    }

                    OnEditComplete?.Invoke(Product);
                }
            });
        }

        public Product Product { get; private set; }

        public RelayCommand ApplyCommand { get; private set; }

        private string _oldName;
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_oldName == value)
                {
                    _name = _oldName;
                    _nameIsDirty = false;
                }
                else
                {
                    _oldName = _name;
                    _name = value;
                    _nameIsDirty = true;
                }
                OnPropertyChanged();
            }
        }
        private bool _nameIsDirty;

        private string _oldDescription;
        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                if (_oldDescription == value)
                {
                    _description = _oldDescription;
                    _descriptionIsDirty = false;
                }
                else
                {
                    _oldDescription = _description;
                    _description = value;
                    _descriptionIsDirty = true;
                }
                OnPropertyChanged();
            }
        }
        private bool _descriptionIsDirty;

        private string _oldPath;
        private string _pathToImage;
        public string PathToImage
        {
            get => _pathToImage;
            set
            {
                if (_oldPath == value)
                {
                    _pathToImage = _oldPath;
                    _pathIsDirty = false;
                }
                else
                {
                    _oldPath = _pathToImage;
                    _pathToImage = value;
                    _pathIsDirty = true;
                }
                OnPropertyChanged();
            }
        }
        private bool _pathIsDirty;

        private decimal _oldPrice;
        private decimal _price;
        public decimal Price
        {
            get => _price;
            set
            {
                if (_oldPrice == value)
                {
                    _price = _oldPrice;
                    _priceIsDirty = false;
                }
                else
                {
                    _oldPrice = _price;
                    _price = value;
                    _priceIsDirty = true;
                }
                OnPropertyChanged();
            }
        }
        private bool _priceIsDirty;

        private byte _oldPercent;
        private byte _discountPercent;
        public byte DiscountPercent
        {
            get => _discountPercent;
            set
            {
                if (_oldPercent == value)
                {
                    _discountPercent = _oldPercent;
                    _percentIsDirty = false;
                }
                else
                {
                    _oldPercent = _discountPercent;
                    _discountPercent = value;
                    _percentIsDirty = true;
                }
                OnPropertyChanged();
            }
        }
        private bool _percentIsDirty;

        private short _oldQuantity;
        private short _quantity;
        public short Quantity
        {
            get => _quantity;
            set
            {
                if (_oldQuantity == value)
                {
                    _quantity = _oldQuantity;
                    _quantityIsDirty = false;
                }
                else
                {
                    _oldQuantity = _quantity;
                    _quantity = value;
                    _quantityIsDirty = true;
                }
                OnPropertyChanged();
            }
        }
        private bool _quantityIsDirty;

        private bool HasDirty => _nameIsDirty || _descriptionIsDirty || _pathIsDirty || _priceIsDirty || _percentIsDirty || _quantityIsDirty;

        public EditProductsVM(Product product) : this()
        {
            Title = $"Редактор продукта {product.Name} ✏";
            Product = product;
            OnPropertyChanged(nameof(Product));
            Dispatcher.CurrentDispatcher.Invoke(Initialize);
        }

        private void Initialize()
        {
            using (BookClubEntities db = new BookClubEntities())
            {
                Name = Product.Name;
                _nameIsDirty = false;
                Description = Product.Description;
                _descriptionIsDirty = false;
                PathToImage = Product.PathToImage;
                _pathIsDirty = false;
                Price = Product.Price;
                _priceIsDirty = false;
                DiscountPercent = Product.DiscountPercent;
                _percentIsDirty = false;
                Quantity = Product.Quantity;
                _quantityIsDirty = false;
            }
        }
    }
}
