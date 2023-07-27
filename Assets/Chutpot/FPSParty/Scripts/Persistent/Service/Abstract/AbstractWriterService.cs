using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chutpot.FPSParty.Persistent
{
    public abstract class AbstractWriterService
    {
        protected readonly string _path;

        protected readonly string _fileLocation;

        private const string _key = "WKK6EHbfgpUef7E/t6tVBg==";
        private const string _iv  = "luEfah/jm2i4yByJTtG7lw==";



        public AbstractWriterService() 
        {
            _path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _fileLocation = _path + "\\ChutPot\\FPSParty\\";
        }

        [PostConstruct]
        public virtual void Initialize() 
        {
            InitializeFile();
        }

        protected abstract void InitializeFile();
        public abstract void Write();
        protected abstract void WriteDefault();

        protected static byte[] EncryptStringToBytes_Aes(string plainText)
        {
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.Zeros;
                aesAlg.Key = Convert.FromBase64String(_key);
                aesAlg.IV = Convert.FromBase64String(_iv);

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        protected static string DecryptStringFromBytes_Aes(string cipherText)
        {
            string plaintext = null;
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.Zeros;
                aesAlg.Key = Convert.FromBase64String(_key);
                aesAlg.IV = Convert.FromBase64String(_iv);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
