using System.Windows;

namespace BookClubApp
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow();
            MainWindow.Show();
            base.OnStartup(e);
        }
    }
}
