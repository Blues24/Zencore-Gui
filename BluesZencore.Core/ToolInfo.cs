namespace BluesZencore.Core
{
    public class ToolInfo
    {
        public string Name { get; set; } = "";
        public string? Path { get; set; }
        public bool IsBundled { get; set; }
        public bool Exists => !string.IsNullOrEmpty(Path);
    }
}