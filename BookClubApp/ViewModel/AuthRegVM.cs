using BookClubApp.Core;
using BookClubApp.Model.Database;
using BookClubApp.Model.Security;
using System;
using System.Data.Entity;
using System.Security;
using System.Threading.Tasks;
using System.Windows;

namespace BookClubApp.ViewModel
{
    public sealed class AuthRegVM : BaseVM
    {
        public event Action OnAuth = delegate { };

        public RelayCommand AuthCommand { get; private set; }

        private string _login;
        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged();
            }
        }

        private SecureString _password;
        public SecureString Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private SecureString _passwordConfirm;
        public SecureString PasswordConfirm
        {
            get => _passwordConfirm;
            set
            {
                _passwordConfirm = value;
                OnPropertyChanged();
            }
        }

        public bool ValuesIsValid
        {
            get
            {
                if (_password == null || _passwordConfirm == null)
                    return false;

                bool hasEmpty = string.IsNullOrWhiteSpace(_login) && _password.Length == 0 && _passwordConfirm.Length == 0;
                bool confirmed = _password.ToUnsecuredString() == _passwordConfirm.ToUnsecuredString();
                return !hasEmpty && confirmed;
            }
        }

        public AuthRegVM()
        {
            Title = "Авторизуйтесь 🥷";

            AuthCommand = new RelayCommand(async _ => await AuthorizationFunc(), __ => ValuesIsValid);
        }

        private void DisposePasswords()
        {
            Password.Dispose();
            PasswordConfirm.Dispose();
        }

        private void Complete()
        {
            DisposePasswords();
            OnAuth.Invoke();
        }

        private async Task AuthorizationFunc()
        {
            using (BookClubEntities db = new BookClubEntities())
            {
                Client client = await db.Client.AsNoTracking().FirstOrDefaultAsync(c => c.Login == _login);

                if (client != null)
                {
                    if (client.Password != _password.ToUnsecuredString())
                    {
                        MessageBox.Show(
                            "Введённый пароль оказался не верным!",
                            "Неверные авторизационные данные",
                            MessageBoxButton.OK, MessageBoxImage.Error);

                        DisposePasswords();
                        Password = new SecureString();
                        PasswordConfirm = new SecureString();
                        return;
                    }
                    else
                        MainVM.SetClient(client);
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show(
                            "Пользователь не существует!",
                            "Пользователь с такими авторизационными данными не найден!"
                            + Environment.NewLine + Environment.NewLine
                            + "Зарегистрировать нового пользователя?",
                            MessageBoxButton.YesNo, MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        client = new Client()
                        {
                            Login = _login,
                            Password = _password.ToUnsecuredString(),
                            PositionID = 1
                        };

                        db.Client.Add(client);
                        await db.SaveChangesAsync();
                    }
                }

                Complete();
            }
        }
    }
}
