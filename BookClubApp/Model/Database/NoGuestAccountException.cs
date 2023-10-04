namespace BookClubApp.Model.Database
{
    /// <summary>
    /// Исключение, которое выдаётся когда требуется наличие аккаунта,
    /// но пользователь не авторизован, <strong>а гостевого аккаунта нет в базе данных</strong>
    /// </summary>
    public sealed class NoGuestAccountException : System.InvalidOperationException
    {
        public const string MSG = "Отсутствует гостевой аккаунт";

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="NoGuestAccountException"/>
        /// </summary>
        public NoGuestAccountException() : base(MSG) { }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="NoGuestAccountException"/> с дополнением к сообщению об ошибке
        /// </summary>
        public NoGuestAccountException(string message) : base(string.Format("{0}. {1}", message, MSG)) { }
    }
}
