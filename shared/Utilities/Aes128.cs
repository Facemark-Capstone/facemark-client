// David Wahid
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace shared.Utilities
{
    public static class Aes128
    {
        /// <summary>
        /// encryptionKey must be 16 characters
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="encryptionKey"></param>
        /// <returns></returns>
        public static string EncryptToUrlSafeBase64String(string plainText, string encryptionKey)
        {
            byte[] encryptionKeyBytes = System.Text.Encoding.UTF8.GetBytes(encryptionKey);
            Debug.Assert(encryptionKeyBytes.Length == 16);

            using (var aes = Aes.Create())
            using (var encryptor = aes.CreateEncryptor(encryptionKeyBytes, aes.IV))
            {
                return EncryptTextInternal(plainText, encryptor, aes.IV);
            }
        }

        /// <summary>
        /// encryptionKey and iv must be 16 characters each
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="encryptionKey">Must be 16 characters</param>
        /// <param name="iv">Must be 16 in length</param>
        /// <returns></returns>
        public static string EncryptToUrlSafeBase64StringWithIV(string plainText, string encryptionKey, byte[] iv)
        {
            byte[] encryptionKeyBytes = System.Text.Encoding.UTF8.GetBytes(encryptionKey);
            Debug.Assert(encryptionKeyBytes.Length == 16);

            using (var aes = Aes.Create())
            using (var encryptor = aes.CreateEncryptor(encryptionKeyBytes, iv))
            {
                return EncryptTextInternal(plainText, encryptor, iv);
            }
        }

        internal static string EncryptTextInternal(string text, ICryptoTransform encryptor, byte[] iv)
        {
            using (var outStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(outStream, encryptor, CryptoStreamMode.Write))
                using (var streamWriter = new StreamWriter(cryptoStream))
                {
                    streamWriter.Write(text);
                }

                Debug.Assert(iv.Length == 16);

                var encryptedBytes = outStream.ToArray();
                var result = new byte[iv.Length + encryptedBytes.Length];

                Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                Buffer.BlockCopy(encryptedBytes, 0, result, iv.Length, encryptedBytes.Length);

                return Encoding.Convert.ToUrlSafeBase64String(result);
            }
        }

        /// <summary>
        /// encryptionKey must be 16 characters
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="encryptionKey"></param>
        /// <returns></returns>
        public static string DecryptFromUrlSafeBase64String(string cipherText, string encryptionKey)
        {
            byte[] encryptionKeyBytes = System.Text.Encoding.UTF8.GetBytes(encryptionKey);
            Debug.Assert(encryptionKeyBytes.Length == 16);

            var cipherBytes = Encoding.Convert.FromUrlSafeBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[cipherBytes.Length - 16];

            Buffer.BlockCopy(cipherBytes, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(cipherBytes, iv.Length, cipher, 0, cipher.Length);

            using (var aes = Aes.Create())
            using (var decryptor = aes.CreateDecryptor(encryptionKeyBytes, iv))
            using (var memoryStream = new MemoryStream(cipher))
            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            using (var streamReader = new StreamReader(cryptoStream))
            {
                return streamReader.ReadToEnd();
            }
        }

        /// <summary>
        /// Encode plainText to base64 string before encrypting it with Aes128
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="encryptionKey">Must be 16 characters</param>
        /// <param name="iv">Must be 16 characters</param>
        /// <returns>Should be decrypted with DecryptAndDecodeFromUrlSafeBase64String</returns>
        public static string EncodeAndEncryptToUrlSafeBase64String(string plainText, string encryptionKey, byte[] iv)
        {
            string base64String = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plainText));

            return EncryptToUrlSafeBase64StringWithIV(base64String, encryptionKey, iv);
        }

        /// <summary>
        /// Decrypt cipherText then decode it from base64 string
        /// </summary>
        /// <param name="cipherText">Should be encrypted with EncodeAndEncryptToUrlSafeBase64String</param>
        /// <param name="encryptionKey">Must be 16 characters</param>
        /// <returns></returns>
        public static string DecryptAndDecodeFromUrlSafeBase64String(string cipherText, string encryptionKey)
        {
            string decryptedBase64String = DecryptFromUrlSafeBase64String(cipherText, encryptionKey);

            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(decryptedBase64String));
        }
    }
}
