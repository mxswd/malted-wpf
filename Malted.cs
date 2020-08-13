using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MaltedWPF
{
    public static class Malted
    {
        public static bool EncryptFile(string filename, string password)
        {
            var deriveKey = new Rfc2898DeriveBytes(password, 8, 1_000_000, HashAlgorithmName.SHA256);

            string outputFileName = $"{filename}.malted";
            if (!(File.Exists(filename) && !File.Exists(outputFileName)))
            {
                return false;
            }
            FileStream inputStream = File.OpenRead(filename);
            FileStream fileOutputStream = File.Create(outputFileName);
            
            var aescng = new AesCng();
            byte[] tmpKey = deriveKey.GetBytes(32 + 16);
            byte[] key = new byte[32];
            byte[] iv = new byte[16];
            Array.Copy(tmpKey, key, 32);
            Array.Copy(tmpKey, 32, iv, 0, 16);
            aescng.Padding = PaddingMode.PKCS7;
            aescng.KeySize = 256;
            aescng.BlockSize = 128;
            aescng.Mode = CipherMode.CBC;
            var encryptor = aescng.CreateEncryptor(key, iv);
            fileOutputStream.Write(Encoding.UTF8.GetBytes("Malted__"));
            fileOutputStream.Write(deriveKey.Salt);
            using (CryptoStream csEncrypt = new CryptoStream(fileOutputStream, encryptor, CryptoStreamMode.Write))
            {
                inputStream.CopyTo(csEncrypt);
                csEncrypt.FlushFinalBlock();
            }
            inputStream.Close();
            fileOutputStream.Close();
            return true;
        }

        public static bool DecryptFile(string filename, string password)
        {
            FileStream file = File.OpenRead(filename);
            string outputFileName = $"{filename}.decrypted";
            if (!(File.Exists(filename) && !File.Exists(outputFileName)))
            {
                return false;
            }

            // read magic first
            byte[] buff = new byte[8];
            int read = file.Read(buff, 0, 8);
            bool returnValue;
            if (Encoding.UTF8.GetString(buff) != "Malted__")
            {
                returnValue = false;

            } else
            {
                byte[] salt = new byte[8];
                read = file.Read(salt, 0, 8);

                var deriveKey = new Rfc2898DeriveBytes(password, salt, 1_000_000, HashAlgorithmName.SHA256);
                byte[] tmpKey = deriveKey.GetBytes(32 + 16);
                byte[] key = new byte[32];
                byte[] iv = new byte[16];
                Array.Copy(tmpKey, key, 32);
                Array.Copy(tmpKey, 32, iv, 0, 16);

                var aescng = new AesCng();
                aescng.Mode = CipherMode.CBC;
                aescng.KeySize = 256;
                aescng.BlockSize = 128;
                aescng.Padding = PaddingMode.PKCS7;
                FileStream fileOutputStream = File.Create(outputFileName);
                var decryptor = aescng.CreateDecryptor(key, iv);

                using (CryptoStream csDecrypt = new CryptoStream(file, decryptor, CryptoStreamMode.Read))
                {
                    csDecrypt.CopyTo(fileOutputStream);
                }
                returnValue = true;
                fileOutputStream.Close();
            }

            file.Close();

            return returnValue;
        }
    }

}
