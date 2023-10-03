using BookClubApp.Core;

namespace BookClubApp.ViewModel
{
    public abstract class BaseVM : ObservableObject
    {
        public string Title { get; set; }
    }
}
