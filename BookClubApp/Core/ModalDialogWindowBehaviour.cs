using System.Windows;
using System.Windows.Controls;

namespace BookClubApp.Core
{
    public static class ModalDialogWindowBehaviour
    {
        public static readonly DependencyProperty ModalDialogWindowProperty =
            DependencyProperty.RegisterAttached("ModalDialogWindow",
                                                typeof(object),
                                                typeof(ModalDialogWindowBehaviour),
                                                new PropertyMetadata(null, OnModalDialogWindowChanged));

        public static void SetModalDialogWindow(DependencyObject d, object value) => d.SetValue(ModalDialogWindowProperty, value);
        public static object GetModalDialogWindow(DependencyObject d) => d.GetValue(ModalDialogWindowProperty);

        private static void OnModalDialogWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ContentControl)
            {
                if (e.NewValue != null)
                {
                    object resource = Application.Current.TryFindResource(e.NewValue.GetType());
                    if (resource is Window window)
                    {
                        window.DataContext = e.NewValue;
                        window.ShowDialog();
                    }
                }
            }
        }
    }
}
