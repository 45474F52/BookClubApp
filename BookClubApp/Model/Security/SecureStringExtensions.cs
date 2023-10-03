using System;
using System.Security;
using System.Runtime.InteropServices;

namespace BookClubApp.Model.Security
{
    internal static class SecureStringExtensions
    {
        /// <summary>
        /// Преобразует зашифрованную строку в обычную
        /// </summary>
        /// <returns>Возвращает расшифрованный текст или <see cref="string.Empty"/>, если зашифрованная строка была пустой или равной null</returns>
        internal static string ToUnsecuredString(this SecureString secureString)
        {
            if (secureString == null || secureString.Length == 0)
            {
                return string.Empty;
            }

            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(ptr);
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(ptr);
                }
            }
        }

        /// <summary>
        /// Зашифровывает переданную строку <paramref name="password"/>
        /// </summary>
        /// <param name="password">Текст, который нужно зашифровать</param>
        /// <remarks>
        /// Метод создаёт новый экзепляр класса <see cref="SecureString"/>, даже при передаче пустого значения <paramref name="password"/>
        /// </remarks>
        internal static unsafe void ToSecuredString(this SecureString secureString, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                secureString = new SecureString();
            }
            else
            {
                fixed (char* ptr = password)
                {
                    char* value = ptr;
                    secureString = new SecureString(value, password.Length);
                }
            }
        }
    }
}
