namespace BluesZencore.Core
{
    public class ArchiveOptions
    {
        public List<string> InputPaths { get; set; } = new();
        public string OutputPath { get; set; } = "output";
        public string Format { get; set; } = "zip";
        public string? Password { get; set; } = null;
        public bool UseCompression { get; set; } = true;
        public bool EnableEncryption { get; set; } = false;

        // Untuk advanced mode
        // Coming soon
        public bool GenerateChecksum { get; set; } = false;
        public Dictionary<string, string>? CustomMetaData { get; set; } = null;
        public bool AdvancedMode =>
            EnableEncryption || GenerateChecksum || (CustomMetaData?.Any() ?? false);

    }
}