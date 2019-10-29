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
