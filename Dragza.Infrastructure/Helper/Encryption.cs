using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Helper
{
    public static class Encryption
    {

        public static string EncryptString(string text, string keyString = "E546C8DF278CD5931069B522E695D1L9")
        {
            var key = new byte[32];
            var keyBytes = Encoding.UTF8.GetBytes(keyString);
            Array.Copy(keyBytes, key, Math.Min(key.Length, keyBytes.Length));

            var iv = new byte[16];
            Array.Copy(Encoding.UTF8.GetBytes("E546C8AF278CD5931069F522E695S1M4"), iv, Math.Min(iv.Length, 16));

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        public static string DecryptString(string cipherText, string keyString = "E546C8DF278CD5931069B522E695D1L9")
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;
            try
            {
                var key = new byte[32];
                var keyBytes = Encoding.UTF8.GetBytes(keyString);
                Array.Copy(keyBytes, key, Math.Min(key.Length, keyBytes.Length));

                var iv = new byte[16];
                Array.Copy(Encoding.UTF8.GetBytes("E546C8AF278CD5931069F522E695S1M4"), iv, Math.Min(iv.Length, 16));

                var fullCipher = Convert.FromBase64String(cipherText);

                using (var aesAlg = Aes.Create())
                {
                    aesAlg.Key = key;
                    aesAlg.IV = iv;

                    using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                    {
                        using (var msDecrypt = new MemoryStream(fullCipher))
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception) { return cipherText; }
        }


        public static string EncryptCriticalString(string text, string keyString = "E546C8DF278CD5931069B522E695D1L9")
        {
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        public static string DecryptCriticalString(string cipherText, string keyString = "E546C8DF278CD5931069B522E695D1L9")
        {
            try
            {
                var fullCipher = Convert.FromBase64String(cipherText);

                var iv = new byte[16];
                var cipher = new byte[fullCipher.Length - iv.Length];

                Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length);

                var key = Encoding.UTF8.GetBytes(keyString);

                using (var aesAlg = Aes.Create())
                {
                    using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                    {
                        string result;
                        using (var msDecrypt = new MemoryStream(cipher))
                        {
                            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (var srDecrypt = new StreamReader(csDecrypt))
                                {
                                    result = srDecrypt.ReadToEnd();
                                }
                            }
                        }

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string EncryptStringForIntercome(int clientId)
        {
            using (var hash = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes("QPFBWhlGuzy8M3Ich_dqQjFmaya6c74FO_v4u4kk")))
            {
                return BitConverter.ToString(hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(clientId.ToString()))).Replace("-", "").ToLower();
            }
        }

        public static string OneWayEnc(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }

}
