using System.Security.Cryptography;

namespace CustomTLV;

public class AssymentricEncryptor
{
    public static (byte[] privateKey, byte[] publicKey) GenerateKeys()
    {
        using var rsa = new RSACryptoServiceProvider(2048);
        return (rsa.ExportRSAPrivateKey(), rsa.ExportRSAPublicKey());
    }

    public static byte[] Encrypt(byte[] data, byte[] publicKey)
    {
        using var rsa = new RSACryptoServiceProvider(2048);
        rsa.ImportRSAPublicKey(publicKey, out _);
        return rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
    }

    public static byte[] Decrypt(byte[] data, byte[] privateKey)
    {
        using var rsa = new RSACryptoServiceProvider(2048);
        rsa.ImportRSAPrivateKey(privateKey, out _);
        return rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
    }
}

public class SymmetricEncryptor
{
    public static byte[] GenerateKey()
    {
        using var aes = new AesCryptoServiceProvider();
        aes.GenerateKey();
        return aes.Key;
    }

    public static byte[] Encrypt(byte[] data, byte[] key)
    {
        using var aes = new AesCryptoServiceProvider();
        aes.Key = key;
        aes.GenerateIV();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        cs.Write(data, 0, data.Length);
        cs.FlushFinalBlock();
        return aes.IV.Concat(ms.ToArray()).ToArray();
    }

    public static byte[] Decrypt(byte[] data, byte[] key)
    {
        using var aes = new AesCryptoServiceProvider();
        aes.Key = key;
        aes.IV = data.Take(16).ToArray();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write);
        cs.Write(data, 16, data.Length - 16);
        cs.FlushFinalBlock();
        return ms.ToArray();
    }
}