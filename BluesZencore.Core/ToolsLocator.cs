using System;
using System.IO;
using System.Runtime.InteropServices;

namespace BluesZencore.Core
{
    public static class ToolLocator
    {
        private static readonly string ToolDir = Path.Combine(AppContext.BaseDirectory, "assets", "tools");

        public static string GetToolPath(string toolName)
        {
            string osToolName = GetOsSpecificToolName(toolName);

            // Cari di global PATH 
            if (IsToolInPath(osToolName, out var globalPAth))
                return globalPAth;

            // cari di folder bundle internal
            string localTool = Path.Combine(ToolDir, osToolName);
            if (File.Exists(localTool))
                return localTool;

            throw new FileNotFoundException($"[X] Tool '{toolName}' tidak ditemukan di sistem atau di bundle.");
        }

        private static string GetOsSpecificToolName(string baseName)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return baseName + ".exe";
            return baseName;
        }

        private static bool IsToolInPath(string toolName, out string foundPath)
        {
            var paths = (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator);
            foreach (var path in paths)
            {
                var fullPath = Path.Combine(path.Trim(), toolName);
                if (File.Exists(fullPath))
                {
                    foundPath = fullPath;
                    return true;
                }
            }
            foundPath = "";
            return false;
        }
    }
}