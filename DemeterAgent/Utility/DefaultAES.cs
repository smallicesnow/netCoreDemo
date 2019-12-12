using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DemeterBackend.Business.Utility
{
    /// <summary>
    /// 用来加密DemeterAgent通信消息
    /// </summary>
    public class DefaultAES
    {
        private const string Salt = "xxxxx";
        private const string Key = "xxxxx";

        public static string AesEncrypt(string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str)) return null;
                Byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(str);

                byte[] encryptedBytes = null;

                // Set your salt here, change it to meet your flavor:
                // The salt bytes must be at least 8 bytes.
                byte[] saltBytes = Encoding.UTF8.GetBytes(Salt);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (var aes = Aes.Create())
                    {
                        aes.KeySize = 256;
                        aes.BlockSize = 128;

                        var encryptKey = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(Key), saltBytes, 1000);
                        aes.Key = encryptKey.GetBytes(aes.KeySize / 8);
                        aes.IV = encryptKey.GetBytes(aes.BlockSize / 8);

                        aes.Mode = CipherMode.CBC;

                        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                            cs.Dispose();
                        }
                        encryptedBytes = ms.ToArray();
                    }
                }

                return Convert.ToBase64String(encryptedBytes);
            }
            catch (Exception)
            {
                return str;
            }
        }

        public static string AesDecrypt(string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str)) return null;
                Byte[] bytesToBeDecrypted = Convert.FromBase64String(str);

                byte[] decryptedBytes = null;

                // Set your salt here, change it to meet your flavor:
                // The salt bytes must be at least 8 bytes.
                byte[] saltBytes = Encoding.UTF8.GetBytes(Salt);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (var aes = Aes.Create())
                    {
                        aes.KeySize = 256;
                        aes.BlockSize = 128;

                        var encryptKey = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(Key), saltBytes, 1000);
                        aes.Key = encryptKey.GetBytes(aes.KeySize / 8);
                        aes.IV = encryptKey.GetBytes(aes.BlockSize / 8);

                        aes.Mode = CipherMode.CBC;

                        using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                            cs.Dispose();
                        }
                        decryptedBytes = ms.ToArray();
                    }
                }

                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (Exception)
            {
                return str;
            }
        }
    }
}
