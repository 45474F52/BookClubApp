using BookClubApp.Core;

namespace BookClubApp.ViewModel
{
    public sealed class HomeVM : BaseVM
    {
        public RelayCommand ShowProductsCommand { get; private set; }
        public RelayCommand OrderCommand { get; private set; }
        public RelayCommand AuthorizeCommand { get; private set; }

        public HomeVM()
        {
            Title = "Стартовая страница 📚";

            ShowProductsCommand = new RelayCommand(_ => { }, __ => true);
            OrderCommand = new RelayCommand(_ => { }, __ => true);
            AuthorizeCommand = new RelayCommand(_ => { }, __ => false);
        }
    }
}
