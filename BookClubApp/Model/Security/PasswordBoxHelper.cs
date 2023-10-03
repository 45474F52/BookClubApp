using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace BookClubApp.Model.Security
{
    /// <summary>
    /// Предоставляет свойства для привязки <see cref="PasswordBox.Password"/> с <see cref="SecureString"/>
    /// </summary>
    internal static class PasswordBoxHelper
    {
        public static SecureString GetSecurePassword(PasswordBox passwordBox) =>
            passwordBox.GetValue(SecurePasswordBindingProperty) as SecureString;

        public static void SetSecurePassword(PasswordBox passwordBox, SecureString secureString) =>
            passwordBox.SetValue(SecurePasswordBindingProperty, secureString);

        private static readonly DependencyProperty _passwordBindingMarshallerProperty = DependencyProperty.RegisterAttached(
            "PasswordBindingMarshaller",
            typeof(PasswordBindingMarshaller),
            typeof(PasswordBoxHelper),
            new PropertyMetadata());

        public static readonly DependencyProperty SecurePasswordBindingProperty = DependencyProperty.RegisterAttached(
            "SecurePassword",
            typeof(SecureString),
            typeof(PasswordBoxHelper),
            new FrameworkPropertyMetadata(new SecureString(),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                AttachedPropertyValueChanged));

        /// <summary>
        /// Создаёт, по необходимости, <see cref="PasswordBindingMarshaller"/>, для возможности обновления пароля и его считывания
        /// </summary>
        private static void AttachedPropertyValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = (PasswordBox)d;

            if (!(passwordBox.GetValue(_passwordBindingMarshallerProperty) is PasswordBindingMarshaller bindingMarshaller))
            {
                bindingMarshaller = new PasswordBindingMarshaller(passwordBox);
                passwordBox.SetValue(_passwordBindingMarshallerProperty, bindingMarshaller);
            }

            bindingMarshaller.UpdatePasswordBox(e.NewValue as SecureString);
        }
    }
}
