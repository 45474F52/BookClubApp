namespace BookClubApp.View
{
    public partial class ProductsView
    {
        public ProductsView() => InitializeComponent();

        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) =>
            (DataContext as ViewModel.ProductsVM).OrderCommand.Execute((sender as System.Windows.Controls.ListViewItem).Content);
    }
}
