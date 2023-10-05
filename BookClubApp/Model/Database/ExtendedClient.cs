namespace BookClubApp.Model.Database
{
    public partial class Client
    {
        public string PathToIcon
        {
            get
            {
                switch (PositionID)
                {
                    case 2:
                        return "Resources/Images/UserIcons/Client.jpeg";
                    case 3:
                        return "Resources/Images/UserIcons/Manager.jpeg";
                    case 4:
                        return "Resources/Images/UserIcons/Administrator.jpeg";
                    case 1:
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
