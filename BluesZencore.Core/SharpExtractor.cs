using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;

public class SharpExtractor : IArchiveExtractor
{
    public void Extract(string archivePath, string destination)
    {
        using var archive = ArchiveFactory.Open(archivePath);
        foreach (var entry in archive.Entries.Where(e => !e.IsDirectory))
        {
            entry.WriteToDirectory(destination, new ExtractionOptions
            {
                ExtractFullPath = true,
                Overwrite = true
            });
        }
    }
}
