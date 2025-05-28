using System.Security.AccessControl;
using System.Text.RegularExpressions;
using BluesZencore.Core;
public static class ArchiveValidator
{
    private static readonly HashSet<string> supportedFormats = new()
    {
        "zip", "tar.gz", "tar.zst", "7z", "rar"
    };
    public static string DetectArchiveFormat(string archivePath)
    {
        if (string.IsNullOrWhiteSpace(archivePath))
            throw new ArgumentException("Path tidak valid");

        string ext = Path.GetExtension(archivePath).ToLowerInvariant();
        string extLower = archivePath.ToLowerInvariant();

        if (extLower.EndsWith(".tar.gz"))
            return "tar.gz";
        if (extLower.EndsWith(".tar.zst"))
            return "tar.zst";

        return ext switch
        {
            ".zip" => "zip",
            ".7z" => "7z",
            ".rar" => "rar",
            ".tar" => "tar",
            ".gz" => "gz",
            ".zst" => "zst",
            _ => throw new NotSupportedException($"Format arsip tidak dikenali: {ext}")
        };
    }

    public static void Validate(ArchiveOptions options)
    {
        if (options.InputPaths == null || options.InputPaths.Count == 0)
        {
            throw new ArgumentException("Input path tidak boleh kosong. ");
        }

        foreach (var path in options.InputPaths)
        {
            if (!File.Exists(path) && !Directory.Exists(path))
            {
                throw new FileNotFoundException("Path tidak ditemukan.");
            }
        }

        if (string.IsNullOrWhiteSpace(options.Format))
            throw new ArgumentException("Format arsip tidak boleh kosong.");

        if (!supportedFormats.Contains(options.Format.ToLower()))
            throw new NotSupportedException($"Format tidak didukung: {options.Format}");

        if (string.IsNullOrWhiteSpace(options.OutputPath))
            throw new ArgumentException("Output path tidak boleh kosong.");

        if (!IsValidVolumeSize(options.VolumeSize))
            throw new ArgumentException("Ukuran volume tidak valid. Gunakan format seperti 100m, 700m, atau 2g.");

        if (!string.IsNullOrWhiteSpace(options.ChecksumAlgorithm))
        {
            var algo = options.ChecksumAlgorithm.ToUpper();
            if (algo != "MD5" && algo != "SHA1" && algo != "SHA256")
                throw new NotSupportedException($"Algorithma tidak didukung: {algo}");
        }
    }

    private static bool IsValidVolumeSize(string? volume)
    {
        if (string.IsNullOrWhiteSpace(volume)) return true;

        var regex = new Regex(@"^\d+(m|g)$", RegexOptions.IgnoreCase);
        return regex.IsMatch(volume);
    }
    private static readonly char[] IllegalChar = Path.GetInvalidFileNameChars();

    public static string SanitizeOutputPath(string outputPath)
    {
        if (!File.Exists(outputPath)) return outputPath;

        string directory = Path.GetDirectoryName(outputPath) ?? ".";
        string filename = Path.GetFileNameWithoutExtension(outputPath);
        string ext = Path.GetExtension(outputPath);
        string newPath;
        int counter = 1;

        do
        {
            newPath = Path.Combine(directory, $"{filename}({counter}){ext}");
            counter++;
        } while (File.Exists(newPath));

        return newPath;
    } 
}