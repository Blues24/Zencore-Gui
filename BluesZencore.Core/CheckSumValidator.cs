using System.Security.Cryptography;
using System.Text;
public static class CheckSumValidator
{
    public static bool Validate(string filePath, string expectedChecksum, string algorithm = "SHA-256")
    {
        string Hash = ChecksumGenerator.Generate(filePath, algorithm);
        return Hash.Equals(expectedChecksum, StringComparison.OrdinalIgnoreCase);
    }

    public static string? TryLoadChecksumFile(string checksumPath)
    {
        if (!File.Exists(checksumPath)) return null;

        var content = File.ReadAllText(checksumPath).Trim();

        return content.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
    }
}