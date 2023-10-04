using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace BookClubApp.Model.Security
{
    /// <summary>
    /// Связующее звено между <see cref="PasswordBox"/> и <see cref="PasswordBoxHelper"/>
    /// </summary>
    internal sealed class PasswordBindingMarshaller
    {
        private readonly PasswordBox _passwordBox;
        private bool _isMarshalling;

        /// <summary>
        /// Подписывает <paramref name="passwordBox"/> на событие <see cref="PasswordBox.PasswordChanged"/><br/>
        /// Предоставляет метод для дешифровки строки и запись значения в <see cref="PasswordBox.Password"/>
        /// </summary>
        /// <param name="passwordBox">Объект, в который запишется зашифрованная строка</param>
        public PasswordBindingMarshaller(PasswordBox passwordBox)
        {
            _passwordBox = passwordBox;
            _passwordBox.PasswordChanged += PasswordBoxPasswordChanged;
        }

        /// <summary>
        /// Дешифрует <see cref="SecureString"/> и записывает значение в <see cref="PasswordBox.Password"/>
        /// </summary>
        /// <param name="newPassword">Зашифрованная строка</param>
        internal void UpdatePasswordBox(SecureString newPassword)
        {
            if (!_isMarshalling)
            {
                _isMarshalling = true;
                try
                {
                    _passwordBox.Password = newPassword.ToUnsecuredString();
                }
                finally
                {
                    _isMarshalling = false;
                }
            }
        }

        /// <summary>
        /// Метод, выполняющийся каждый раз при вызове события <see cref="PasswordBox.PasswordChanged"/>
        /// </summary>
        private void PasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!_isMarshalling)
            {
                _isMarshalling = true;
                try
                {
                    PasswordBoxHelper.SetSecurePassword(_passwordBox, _passwordBox.SecurePassword.Copy());
                }
                finally
                {
                    _isMarshalling = false;
                }
            }
        }
    }
}
