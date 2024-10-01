using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Clase para encriptar información. Util para el guardado y carga de datos en un juego.
/// Hecha totalmente por Alex Perez aka Eskemagames.
/// </summary>
public sealed class CryptoString {
    private CryptoString() { }
    private static byte[] savedKey = { 0, 99, 2, 34, 94, 5, 6, 89, 209, 50, 10,103, 120, 201, 114, 15, 16, 200, 254, 19, 20, 21, 90, 99 };
    private static byte[] savedIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF }; 

    public static byte[] Key {
        get { return savedKey; }
        set { savedKey = value; }
    }

    public static byte[] IV {
        get { return savedIV; }
        set { savedIV = value; }
    }

    private static void RdGenerateSecretKey(RijndaelManaged rdProvider) {
        if (savedKey == null) {
            rdProvider.KeySize = 256;
            rdProvider.GenerateKey();
            savedKey = rdProvider.Key;
        }
    }

    private static void RdGenerateSecretInitVector(RijndaelManaged rdProvider) {
        if (savedIV == null) {
            rdProvider.GenerateIV();
            savedIV = rdProvider.IV;
        }
    }

    public static string Encrypt(string originalStr) {
        // Encode data string to be stored in memory.
        byte[] originalStrAsBytes = Encoding.UTF8.GetBytes(originalStr);
        byte[] originalBytes = { };
        // Create MemoryStream to contain output.
        using (MemoryStream memStream = new
                 MemoryStream(originalStrAsBytes.Length)) {
            using (RijndaelManaged rijndael = new RijndaelManaged()) {
                // Generate and save secret key and init vector.
                RdGenerateSecretKey(rijndael);
                RdGenerateSecretInitVector(rijndael);
                if (savedKey == null || savedIV == null) {
                    throw (new NullReferenceException(
                            "savedKey and savedIV must be non-null."));
                }
                // Create encryptor and stream objects.
                using (ICryptoTransform rdTransform =
                       rijndael.CreateEncryptor((byte[])savedKey.
                       Clone(), (byte[])savedIV.Clone())) {
                    using (CryptoStream cryptoStream = new CryptoStream(memStream,
                          rdTransform, CryptoStreamMode.Write)) {
                        // Write encrypted data to the MemoryStream.
                        cryptoStream.Write(originalStrAsBytes, 0,
                                   originalStrAsBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        originalBytes = memStream.ToArray();
                    }
                }
            }
        }
        // Convert encrypted string.
        string encryptedStr = Convert.ToBase64String(originalBytes);
        return (encryptedStr);
    }

    public static string Decrypt(string encryptedStr) {
        // Unconvert encrypted string.
        byte[] encryptedStrAsBytes = Convert.FromBase64String(encryptedStr);
        byte[] initialText = new Byte[encryptedStrAsBytes.Length];
        using (RijndaelManaged rijndael = new RijndaelManaged()) {
            using (MemoryStream memStream = new MemoryStream(encryptedStrAsBytes)) {
                if (savedKey == null || savedIV == null) {
                    throw (new NullReferenceException(
                            "savedKey and savedIV must be non-null."));
                }
                // Create decryptor and stream objects.
                using (ICryptoTransform rdTransform =
                     rijndael.CreateDecryptor((byte[])savedKey.
                     Clone(), (byte[])savedIV.Clone())) {
                    using (CryptoStream cryptoStream = new CryptoStream(memStream,
                     rdTransform, CryptoStreamMode.Read)) {
                        // Read in decrypted string as a byte[].
                        cryptoStream.Read(initialText, 0, initialText.Length);
                    }
                }
            }
        }
        // Convert byte[] to string.
        string decryptedStr = Encoding.UTF8.GetString(initialText);
        return (decryptedStr);
    }
}
