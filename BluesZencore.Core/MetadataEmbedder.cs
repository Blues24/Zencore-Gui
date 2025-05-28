using System.Text.Json;
public static class MetadataEmbedder
{
    public static void GenerateMetadataFile(string archivePath, Dictionary<string, string> metadata)
    {
        string metaPath = archivePath + ".meta.json";
        var json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(metaPath, json);
    }

    public static Dictionary<string, string>? ReadMetadataFile(string archivePath)
    {
        string metaPath = archivePath + ".meta.json";
        if (!File.Exists(metaPath)) return null;

        var json = File.ReadAllText(metaPath);
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json);
    }

    public static bool ValidateMetadata(string archivePath, Dictionary<string, string> expectedMetadata)
    {
        var actual = ReadMetadataFile(archivePath);
        if (actual == null) return false;

        return expectedMetadata.All(kv => actual.TryGetValue(kv.Key, out var val) && val == kv.Value);
    }
}

