using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;
using Microsoft.Extensions.Configuration;

namespace API.Helpers
{
    public class EncryptionHelper
    {
        private static string Key;
        private static string Iv;

        public EncryptionHelper(IConfiguration configuration)
        {
            Key = configuration.GetValue<string>("MessagesEncryptionKey");
            Iv = configuration.GetValue<string>("MessagesEncryptionVector");
        }

        // Encrypt a string
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return null;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Key));
                aesAlg.IV = Encoding.UTF8.GetBytes(Iv);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        // Decrypt a string
        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return null;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Key));
                aesAlg.IV = Encoding.UTF8.GetBytes(Iv);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
