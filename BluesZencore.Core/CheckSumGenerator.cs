using System.ComponentModel;
using System.Security.Cryptography;

public static class ChecksumGenerator
{
    public static string Generate(string filePath, string algorithm = "SHA-256")
    {
        using var stream = new BufferedStream(File.OpenRead(filePath), 1024 * 1024);
        HashAlgorithm Hashing = algorithm.ToUpper() switch
        {
            "MD5" => MD5.Create(),
            "SHA256" => SHA256.Create(),
            "SHA1" => SHA1.Create(),
            "SHA512" => SHA512.Create(),
            _ => throw new ArgumentException($"Unsupported Algorithm: {algorithm}, Pls contact Blues24 on github and make issues about your algorithm")

        };

        byte[] hash = Hashing.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}