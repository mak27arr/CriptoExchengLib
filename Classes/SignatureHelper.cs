using System;
using System.Security.Cryptography;
using System.Text;

namespace CriptoExchengLib.Classes
{
    public class SignatureHelper
    {
        public string CreateTokenBase64(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }

        public static string Sign(string key, string message,int signature_bit = 512)
        {
            if (key == null || message == null || key.Length == 0)
                return "";

            if (signature_bit == 512)
            {
                using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
                {
                    byte[] b = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
                    return ByteToString(b);
                }
            }
            if(signature_bit == 384)
            {
                using (var hmac = new HMACSHA384(Encoding.UTF8.GetBytes(key)))
                {
                    byte[] b = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
                    return ByteToString(b);
                }
            }
            if (signature_bit == 256)
            {
                using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
                {
                    byte[] b = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
                    return ByteToString(b);
                }
            }
            if (signature_bit == 1)
            {
                using (var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(key)))
                {
                    byte[] b = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
                    return ByteToString(b);
                }
            }
            return "";
        }

        public static byte[] Sign(byte[] keyByte, byte[] messageBytes)
        {
            using (var hmacsha512 = new HMACSHA512(keyByte))
            {

                return hmacsha512.ComputeHash(messageBytes);

            }
        }

        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }



        public static byte[] Sha256_hash(String value)
        {
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;

                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                return result;
            }
        }

        private static string ByteToString(byte[] buff)
        {
            string sbinary = "";

            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary).ToLowerInvariant();
        }

    }
}
