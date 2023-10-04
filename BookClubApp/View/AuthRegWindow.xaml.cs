using System.Windows;

namespace BookClubApp.View
{
    public partial class AuthRegWindow : Window
    {
        public AuthRegWindow() => InitializeComponent();

        private void Button_Click(object sender, RoutedEventArgs e) => DialogResult = true;
    }
}
