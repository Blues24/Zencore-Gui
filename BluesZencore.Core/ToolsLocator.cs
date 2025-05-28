using System.Diagnostics;

namespace BluesZencore.Core
{
    public static class ToolLocator
    {
        public static IArchiveExtractor? GetFallbackExtractor(string format)
        {
            
            if (ExtractorManager.Extractors.TryGetValue($"{format}-fallback", out var extractor))
            {
                return extractor;
            }
            
            return null;
        }

        private static readonly Dictionary<string, ToolInfo> tools = new();

        private static readonly string bundledToolDir = Path.Combine(AppContext.BaseDirectory, "assets", "tools");

        private static readonly string[] knownTools = new[]
        {
            "7z", "tar", "unrar", "unzip", "zstd", "xz"
        };

        public static void Scan()
        {
            tools.Clear();
            foreach (var toolName in knownTools)
            {
                string? path = FindInSystem(toolName);
                if (path != null)
                {
                    tools[toolName] = new ToolInfo { Name = toolName, Path = path, IsBundled = false };
                    continue;
                }

                string bundledPath = Path.Combine(bundledToolDir, toolName + GetExeExt());
                if (File.Exists(bundledPath))
                {
                    tools[toolName] = new ToolInfo { Name = toolName, Path = bundledPath, IsBundled = true };
                }
            }
        }

        public static ToolInfo? GetTool(string name)
        {
            tools.TryGetValue(name, out var tool);
            return tool;
        }
        public static string? GetExtractorForFormat(string format)
        {
            format = format.ToLowerInvariant();

            return format switch
            {
                "zip" => GetTool("7z")?.Path ?? GetTool("unzip")?.Path,
                "rar" => GetTool("7z")?.Path ?? GetTool("unrar")?.Path,
                "7z" => GetTool("7z")?.Path,
                "tar" or "tar.gz" or "tgz" or "tar.zst" => GetTool("7z")?.Path ?? GetTool("tar")?.Path,
                _ => null
            };
        }

        private static string? FindInSystem(string toolName)
        {
            try
            {
                string cmd = OperatingSystem.IsWindows() ? "where" : "which";
                using var proc = Process.Start(new ProcessStartInfo
                {
                    FileName = cmd,
                    Arguments = toolName,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                });

                string output = proc?.StandardOutput.ReadLine() ?? "";
                proc?.WaitForExit();

                return string.IsNullOrWhiteSpace(output) ? null : output.Trim();
            }
            catch
            {
                return null;
            }
        }

        private static string GetExeExt() => OperatingSystem.IsWindows() ? ".exe" : "";
    }

}