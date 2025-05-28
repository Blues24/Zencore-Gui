using System.Diagnostics;
public class ExternalExtractor : IArchiveExtractor
{
    private readonly string _toolPath;
    private readonly string _format;

    public ExternalExtractor(string toolPath, string format)
    {
        _toolPath = toolPath;
        _format = format.ToLower();
    }

    public void Extract(string archivePath, string destination)
    {
        string args = _format switch
        {
            "zip" => $"x \"{archivePath}\" -o\"{destination}\" -y",
            "rar" => $"x \"{archivePath}\" \"{destination}\" -y",
            "7z" => $"x \"{archivePath}\" -o\"{destination}\" -y",
            "tar" or "tar.gz" or "tgz" => $"-xf \"{archivePath}\" -C \"{destination}\"",
            "tar.zst" => $"--use-compress-program=unzstd -xf \"{archivePath}\" -C \"{destination}\"",
            _ => throw new NotSupportedException($"Ekstensi '{_format}' tidak dikenali.")
        };

        var psi = new ProcessStartInfo
        {
            FileName = _toolPath,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var proc = Process.Start(psi)!;
        proc.WaitForExit();

        if (proc.ExitCode != 0)
        {
            string error = proc.StandardError.ReadToEnd();
            throw new InvalidOperationException($"Ekstraksi gagal: {error}");
        }
    }
}
