using System.Diagnostics;
using System;

namespace BluesZencore.Core
{
    public static class ExternalArchiver
    {
        public static void Create7zArchive(ArchiveOptions options)
        {
            string archivePath = options.OutputPath.EndsWith(".7z") ? options.OutputPath : options.OutputPath + ".7z";
            string inputPaths = string.Join(" ", options.InputPaths.Select(p => $"\"{p}\""));
            string passwordPart = options.EnableEncryption && !string.IsNullOrWhiteSpace(options.Password)
                ? $"-p\"{options.Password}\" -mhe=on"
                : "";

            string args = $"a \"{archivePath}\" {inputPaths} {passwordPart} -mx=9";

            RunProcess(ToolLocator.GetToolPath("7z"), args);
        }

        public static void CreateRarArchive(ArchiveOptions options)
        {
            string archivePath = options.OutputPath.EndsWith(".rar") ? options.OutputPath : options.OutputPath + ".rar";
            string inputPaths = string.Join(" ", options.InputPaths.Select(p => $"\"{p}\""));
            string passwordPart = options.EnableEncryption && !string.IsNullOrWhiteSpace(options.Password)
                ? $"-p\"{options.Password}\" -hp"
                : "";

            string args = $"a \"{archivePath}\" {inputPaths} {passwordPart} -m5";

            RunProcess(ToolLocator.GetToolPath("rar"), args);
        }
        public static void CreateTarZstArchive(ArchiveOptions options)
        {
            string tarName = options.OutputPath.EndsWith(".tar.zst") ? options.OutputPath : options.OutputPath + ".tar.zst";
            string tempTar = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".tar");
            string inputPaths = string.Join(" ", options.InputPaths.Select(p => $"\"{p}\""));

            RunProcess(ToolLocator.GetToolPath("tar"), $"-cf \"{tempTar}\" {inputPaths}");
            RunProcess(ToolLocator.GetToolPath("zstd"), $"\"{tempTar}\" -o \"{tarName}\"");
            File.Delete(tempTar);
        }
        public static void CreateTarGzArchive(ArchiveOptions options)
        {
            string tarName = options.OutputPath.EndsWith(".tar.gz") ? options.OutputPath : options.OutputPath + ".tar.gz";
            string inputPaths = string.Join(" ", options.InputPaths.Select(p => $"\"{p}\""));
            string args = $"-czf \"{tarName}\" {inputPaths}";

            RunProcess(ToolLocator.GetToolPath("tar"), args);
        }
        private static void RunProcess(string toolPath, string args)
        {
            var psi = new ProcessStartInfo
            {
                FileName = toolPath,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = false,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Console.WriteLine($"> {toolPath} {args}");

            using var proc = Process.Start(psi);
            if (proc == null)
            {
                Console.Error.WriteLine($"Gagal menjalankan Proses: {psi.FileName}");
            }

            proc.WaitForExit();
            
            string output = proc.StandardOutput.ReadToEnd();
            string error = proc.StandardError.ReadToEnd();

            if (!string.IsNullOrWhiteSpace(output))
                Console.WriteLine(output);
            if (!string.IsNullOrWhiteSpace(error))
                Console.Error.WriteLine(error);
        }
        public static void CreateArchiveWithCli(ArchiveOptions options)
        {

            string volumePart = "";
            if (!string.IsNullOrWhiteSpace(options.volumeSize))
            {
                volumePart = $"-v{options.volumeSize}";
            }

            string excludeArgs = "";
            if (options.excludePattern != null && options.excludePattern.Any())
            {
                excludeArgs = string.Join(".", options.excludePattern.Select(p => $"-x!{p}"));
            }
            
            switch (options.Format.ToLower())
            {
                case "zip":
                case "7z":
                    Create7zArchive(options);
                    break;
                case "rar":
                    CreateRarArchive(options);
                    break;
                case "tar.gz":
                    CreateTarGzArchive(options);
                    break;
                case "tar.zst":
                    CreateTarZstArchive(options);
                    break;
                default:
                    System.Console.WriteLine($"[X] Format tidak ada didaftar yang didukung: {options.Format}");
                    break;
            }
        }
    }
}