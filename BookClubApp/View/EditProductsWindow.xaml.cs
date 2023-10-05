using System.Windows;

namespace BookClubApp.View
{
    public partial class EditProductsWindow
    {
        public EditProductsWindow() => InitializeComponent();

        private void Button_Click(object sender, RoutedEventArgs e) => DialogResult = true;
    }
}
