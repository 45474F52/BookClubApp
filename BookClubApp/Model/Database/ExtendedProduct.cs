using System;

namespace BookClubApp.Model.Database
{
    public partial class Product
    {
        public event Action OnToOrderChanged = delegate { };

		private bool _toOrder;
		public bool ToOrder
		{
			get => _toOrder;
			private set
			{
				_toOrder = value;
				OnToOrderChanged.Invoke();
			}
		}

		public string ToOrderString => ToOrder ? "Удалить из заказа" : "Добавить в заказ";

		public void SetToOrder() => ToOrder = !ToOrder;

		public decimal PriceWithDiscount => Price - Price * DiscountPercent;

		public override string ToString() => Name;
    }
}
