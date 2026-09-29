using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DubaiInvestment.PMS.Clr
{
    /// <summary>
    /// AES-256-CBC, PKCS7, UTF-8 text, URL-safe Base64 without padding: the same
    /// output as the web app's EncryptionHelper.EncryptUrlSafe.
    /// Uses AesManaged (fully managed) rather than Aes.Create(), which goes through
    /// the OS crypto provider and is why the original assemblies needed UNSAFE.
    /// </summary>
    internal static class AesUrl
    {
        private static AesManaged CreateAes()
        {
            return new AesManaged
            {
                Key = ClrKeys.Key,
                IV = ClrKeys.IV,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };
        }

        internal static string Encrypt(string plainText)
        {
            byte[] encrypted;
            using (var aes = CreateAes())
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                    sw.Write(plainText);
                encrypted = ms.ToArray();
            }
            return Convert.ToBase64String(encrypted).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        internal static string Decrypt(string urlSafeBase64)
        {
            var text = urlSafeBase64.Replace('-', '+').Replace('_', '/');
            switch (text.Length % 4)
            {
                case 2: text += "=="; break;
                case 3: text += "="; break;
            }
            using (var aes = CreateAes())
            using (var ms = new MemoryStream(Convert.FromBase64String(text)))
            using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs, Encoding.UTF8))
                return sr.ReadToEnd();
        }
    }
}
