public interface IArchiveExtractor
{
    void Extract(string archivePath, string destinationPath);
    //Task<bool> ExtractAsync (string archivePath, string destinationPath, string? password = null);
}