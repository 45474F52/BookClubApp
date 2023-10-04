using BookClubApp.Core;
using BookClubApp.Model.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace BookClubApp.ViewModel
{
    public sealed class EditOrderVM : BaseVM
    {
        public event Action OnEditComplete = delegate { };

        public EditOrderVM() { }

        public Order Order { get; }

        public RelayCommand ApplyCommand { get; private set; }

        public IEnumerable<OrderStatus> Status { get; private set; }

        private OrderStatus _oldStatus;
        private OrderStatus _selectedStatus;
        public OrderStatus SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (_oldStatus?.ID == value.ID)
                {
                    _selectedStatus = _oldStatus;
                    _statusIsDirty = false;
                }
                else
                {
                    _oldStatus = _selectedStatus ?? value;
                    _selectedStatus = value;
                    _statusIsDirty = true;
                }
                OnPropertyChanged();
            }
        }

        private bool _statusIsDirty = false;

        public IEnumerable<OrderPickupPoint> OrderPickupPoints { get; private set; }

        private OrderPickupPoint _oldPoint;
        private OrderPickupPoint _selectedPoint;
        public OrderPickupPoint SelectedPoint
        {
            get => _selectedPoint;
            set
            {
                if (_oldPoint?.ID == value.ID)
                {
                    _selectedPoint = _oldPoint;
                    _pointIsDirty = false;
                }
                else
                {
                    _oldPoint = _selectedPoint ?? value;
                    _selectedPoint = value;
                    _pointIsDirty = true;
                }
                OnPropertyChanged();
            }
        }

        private bool _pointIsDirty = false;

        private byte _oldTime;
        private byte _deliveryTimeInDays;
        public byte DeliveryTimeInDays
        {
            get => _deliveryTimeInDays;
            set
            {
                if (_oldTime == value)
                {
                    _deliveryTimeInDays = _oldTime;
                    _timeIsDirty = false;
                }
                else
                {
                    _oldTime = _deliveryTimeInDays;
                    _deliveryTimeInDays = value;
                    _timeIsDirty = true;
                }
                OnPropertyChanged();
            }
        }

        private bool _timeIsDirty = false;

        private bool HasDirty => _statusIsDirty || _pointIsDirty || _timeIsDirty;

        public EditOrderVM(Order order)
        {
            Title = $"Редактор заказа {order.PickupCode} ✏";
            Order = order;
            OnPropertyChanged(nameof(Order));

            ApplyCommand = new RelayCommand(async _ =>
            {
                if (HasDirty)
                {
                    using (BookClubEntities db = new BookClubEntities())
                    {
                        Order target = db.Order.Find(Order.ID);

                        if (_statusIsDirty)
                            target.StatusID = SelectedStatus.ID;
                        if (_pointIsDirty)
                            target.PickupPointID = SelectedPoint.ID;
                        if (_timeIsDirty)
                            target.DeliveryTimeInDays = DeliveryTimeInDays.ToString();

                        await db.SaveChangesAsync();
                    }

                    OnEditComplete.Invoke();
                }
            });

            Dispatcher.CurrentDispatcher.Invoke(Initialize);
        }

        private void Initialize()
        {
            using (BookClubEntities db = new BookClubEntities())
            {
                Status = db.OrderStatus.AsNoTracking().ToList();
                OrderPickupPoints = db.OrderPickupPoint.AsNoTracking().ToList();

                SelectedStatus = Order.OrderStatus;
                _statusIsDirty = false;
                SelectedPoint = Order.OrderPickupPoint;
                _pointIsDirty = false;
                DeliveryTimeInDays = byte.Parse(Order.DeliveryTimeInDays);
                _timeIsDirty = false;
            }
        }
    }
}
