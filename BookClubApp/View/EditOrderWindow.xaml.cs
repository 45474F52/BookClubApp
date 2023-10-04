using System.Windows;

namespace BookClubApp.View
{
    public partial class EditOrderWindow
    {
        public EditOrderWindow() => InitializeComponent();

        private void Button_Click(object sender, RoutedEventArgs e) => DialogResult = true;
    }
}
