using System.Text.RegularExpressions;
using BluesZencore.Core;
public static class ArchiveValidator
{
    public static List<string> Validate(ArchiveOptions options)
    {
        var errors = new List<string>();

        // 1. Input path harus ada
        if (options.InputPaths == null || !options.InputPaths.Any())
            errors.Add("Setidaknya satu file atau folder harus dipilih.");

        // 2. Semua path input harus eksis
        foreach (var path in options.InputPaths)
        {
            if (!File.Exists(path) && !Directory.Exists(path))
                errors.Add($"Path tidak ditemukan: {path}");
        }

        // 3. Format harus valid
        var allowedFormats = new[] { "zip", "7z", "rar", "tar.gz", "tar.zst" };
        if (!allowedFormats.Contains(options.Format))
            errors.Add($"Format arsip tidak dikenali: {options.Format}");

        // 4. Jika gunakan password, pastikan tidak kosong
        if (options.UsePassword && string.IsNullOrWhiteSpace(options.Password))
            errors.Add("Password tidak boleh kosong jika enkripsi diaktifkan.");

        // 5. Level kompresi antara 0 - 9
        if (options.CompressionLevel < 0 || options.CompressionLevel > 9)
            errors.Add("Level kompresi harus antara 0 (tidak dikompres) sampai 9 (maksimum).");

        // 6. VolumeSize jika ada, harus valid format (mis. 100m, 1g, 700m)
        if (!string.IsNullOrWhiteSpace(options.VolumeSize) &&
            !Regex.IsMatch(options.VolumeSize, @"^\d+(k|m|g)$", RegexOptions.IgnoreCase))
        {
            errors.Add("Ukuran volume harus dalam format seperti 100m, 700m, atau 2g.");
        }

        return errors;
    }
}
