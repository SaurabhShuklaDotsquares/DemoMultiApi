using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EC.Core.LIBS
{
    public class PasswordEncryption
    {
        public static bool IsPasswordMatch(string userPassword, string enteredPassword, string saltKey)
        {
            if (userPassword.HasValue() && enteredPassword.HasValue() && saltKey.HasValue())
            {
                string savedPassword = GenerateEncryptedPassword(enteredPassword, saltKey, "MD5");
                //set fixed Password.
                //savedPassword = "1076E1F0CAE03096CFE165D4CAF3B15B";
                return userPassword.Equals(savedPassword);
            }

            return false;
        }

        /// <summary>
        /// Create salt key
        /// </summary>
        /// <param name="size">Key size</param>
        /// <returns>Salt key</returns>
        public static string CreateSaltKey(int size = 32)
        {
            //generate a cryptographic random number
            using (var provider = new RNGCryptoServiceProvider())
            {
                var buff = new byte[size];
                provider.GetBytes(buff);

                // Return a Base64 string representation of the random number
                return Convert.ToBase64String(buff);
            }
        }

        /// <summary>
        /// Create a password hash
        /// </summary>
        /// <param name="password">{assword</param>
        /// <param name="saltkey">Salk key</param>
        /// <param name="passwordFormat">Password format (hash algorithm)</param>
        /// <returns>Password hash</returns>
        public static string GenerateEncryptedPassword(string password, string saltkey, string passwordFormat = "MD5")
        {
            return CreateHash(Encoding.UTF8.GetBytes(String.Concat(saltkey, password)), passwordFormat);
        }

        /// <summary>
        /// Create a data hash
        /// </summary>
        /// <param name="data">The data for calculating the hash</param>
        /// <param name="hashAlgorithm">Hash algorithm</param>
        /// <returns>Data hash</returns>
        private static string CreateHash(byte[] data, string hashAlgorithm = "MD5")
        {
            if (!hashAlgorithm.HasValue())
            {
                hashAlgorithm = "MD5";
            }

            var algorithm = (HashAlgorithm)CryptoConfig.CreateFromName(hashAlgorithm);
            if (algorithm == null)
            {
                throw new ArgumentException("Unrecognized hash name");
            }

            var hashByteArray = algorithm.ComputeHash(data);
            return BitConverter.ToString(hashByteArray).Replace("-", "");
        }

        #region Utilities

        private byte[] EncryptTextToMemory(string data, byte[] key, byte[] iv)
        {
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, new TripleDESCryptoServiceProvider().CreateEncryptor(key, iv), CryptoStreamMode.Write))
                {
                    byte[] toEncrypt = Encoding.Unicode.GetBytes(data);
                    cs.Write(toEncrypt, 0, toEncrypt.Length);
                    cs.FlushFinalBlock();
                }

                return ms.ToArray();
            }
        }

        private string DecryptTextFromMemory(byte[] data, byte[] key, byte[] iv)
        {
            using (var ms = new MemoryStream(data))
            {
                using (var cs = new CryptoStream(ms, new TripleDESCryptoServiceProvider().CreateDecryptor(key, iv), CryptoStreamMode.Read))
                {
                    using (var sr = new StreamReader(cs, Encoding.Unicode))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }



        #endregion
    }
}
