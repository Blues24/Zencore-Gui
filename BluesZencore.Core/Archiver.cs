using System;
using System.Xml;

namespace BluesZencore.Core
{
    public static class ArchiverHelper
    {
        public static void ApplyArchiveNameTemplate(ArchiveOptions options)
        {
            if (!string.IsNullOrWhiteSpace(options.archiveNameTemplate))
            {
                string filename = TemplateResolver.ResolveTemplate(options.archiveNameTemplate);
                string? dir = Path.GetDirectoryName(options.OutputPath);
                string ext = options.Format;
                options.OutputPath = Path.Combine(dir ?? ".", Path.ChangeExtension(filename, ext));
            }
        }

        public static void ApplyChecksumIfNeeded(ArchiveOptions options)
        {
            if (options.GenerateChecksum)
            {
                string algorithm = options.ChecksumAlgorithm ?? "SHA256";
                string checksum = ChecksumGenerator.Generate(options.OutputPath, algorithm);
                string checksumFile = $"{options.OutputPath}.{algorithm.ToLowerInvariant()}.txt";
                File.WriteAllText(checksumFile, checksum);
            }
        }

        public static void EmbedMetadataIfPresent(ArchiveOptions options)
        {
            if (options.Metadata != null && options.Metadata.Any())
            {
                MetadataEmbedder.GenerateMetadataFile(options.OutputPath, options.Metadata);
            }
        }
    }
    public static class Archiver
    {
        public static void CreateArchive(ArchiveOptions options)
        {
            Console.WriteLine($"Membuat arsip dalam format: {options.Format}");
            bool useCli = options.AdvancedMode;
            // 🌟 1. Terapkan template nama arsip dulu
            ArchiverHelper.ApplyArchiveNameTemplate(options);

            // 🌟 2. Sanitasi path output
            options.OutputPath = ArchiveValidator.SanitizeOutputPath(options.OutputPath);

            // 🌟 3. Jika Advanced Mode aktif, jalankan fitur tambahan
            if (options.AdvancedMode)
            {
                ArchiverHelper.ApplyChecksumIfNeeded(options);
                ArchiverHelper.EmbedMetadataIfPresent(options);
            }

            switch (options.Format.ToLower())
            {
                case "zip":
                case "tar":
                case "tar.gz":
                    if (useCli)
                    {
                        ExternalArchiver.CreateArchiveWithCli(options);
                    }
                    else
                    {
                        SharpArchiver.CreateWithSharpCompress(options);
                    }
                    break;

                case "tar.zst":
                case "7z":
                case "rar":
                    ExternalArchiver.CreateArchiveWithCli(options);
                    break;

                default:
                    Console.WriteLine($"[X] Format tidak ada didaftar: {options.Format}");
                    break;
            }

        }
    }
}