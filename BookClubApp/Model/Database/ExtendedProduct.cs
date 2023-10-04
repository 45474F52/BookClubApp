using BookClubApp.Core;
using System;

namespace BookClubApp.Model.Database
{
    public partial class Product : ObservableObject
    {
        public event Action OnToOrderChanged = delegate { };

        private bool _toOrder;
        public bool ToOrder
        {
            get => _toOrder;
            private set
            {
                _toOrder = value;
                CountInOrder = value ? (short)1 : (short)0;
                OnToOrderChanged.Invoke();
            }
        }

        public string ToOrderString => ToOrder ? "Удалить из заказа" : "Добавить в заказ";

        public void ChangeToOrderFlag() => ToOrder = !ToOrder;

        private decimal SummOfDiscount => (Price * DiscountPercent) / 100;

        public decimal PriceWithDiscount => Price - SummOfDiscount;

        private short _countInOrder;
        public short CountInOrder
        {
            get => _countInOrder;
            set
            {
                _countInOrder = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        public decimal TotalPrice => PriceWithDiscount * CountInOrder;

        public override string ToString() => Name;
    }
}
