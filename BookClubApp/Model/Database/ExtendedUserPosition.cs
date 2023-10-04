namespace BookClubApp.Model.Database
{
    public partial class UserPosition
    {
        public enum Positions : int
        {
            Guest = 1,
            Client = 2,
            Manager = 3,
            Administrator = 4
        }
    }
}
