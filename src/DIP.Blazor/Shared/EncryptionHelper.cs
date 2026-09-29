using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Buffers.Text;

public static class EncryptionHelper
{
    // Replace with your secure key (32 bytes for AES-256)
    private static readonly byte[] Key = Convert.FromBase64String("6J3mUBoVjTy0qwQn1LTh+Kd9H3hQeL4c3uLlMJe+Lfw=");
    private static readonly byte[] IV = Convert.FromBase64String("Yg2aT5e1VQvYlfZCZ0DjqA==");

    public static string EncryptUrlSafe(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
            sw.Write(plainText);

        var encrypted = ms.ToArray();
        return Base64Url.EncodeToString(encrypted); // URL-safe, no '/', '+', '=' :contentReference[oaicite:1]{index=1}
    }

    public static string DecryptUrlSafe(string urlSafeBase64)
    {
        // 1. Decode Base64Url string to byte[]
        byte[] decodedBytes = Base64Url.DecodeFromChars(urlSafeBase64);

        // 2. Decrypt using AES
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        using var ms = new MemoryStream(decodedBytes);
        using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}
