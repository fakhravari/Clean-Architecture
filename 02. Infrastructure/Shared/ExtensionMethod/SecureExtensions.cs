using System.Security.Cryptography;
using System.Text;

namespace Shared.ExtensionMethod
{
    public static class SecureExtensions
    {
        private static string AesKey => "ABC0123456789DEF56789ABC01234DEF";
        private static string AesIV => "AB0123CD456EF789";

        public static string Encrypt(this string text)
        {
            try
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(AesKey);
                    aes.IV = Encoding.UTF8.GetBytes(AesIV);

                    var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(text);
                            }
                        }

                        var encryptedData = msEncrypt.ToArray();
                        return Convert.ToBase64String(encryptedData);
                    }
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string Decrypt(this string encryptedText)
        {
            try
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(AesKey);
                    aes.IV = Encoding.UTF8.GetBytes(AesIV);

                    var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using (var msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText)))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}

