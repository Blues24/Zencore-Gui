using System;
using System.IO;
using SharpCompress.Archives;
using ZstdSharp;
using SharpCompress.Common;
using SharpCompress.Writers;
using SharpCompress.Writers.Tar;
using SharpCompress.Writers.GZip;
using SharpCompress.Writers.Zip;
using SharpCompress.IO;
using System.IO.Compression;

namespace BluesZencore.Core
{
    public static class SharpArchiver
    {
        public static void CreateWithSharpCompress(ArchiveOptions options)
        {
            try
            {
                string archivePath = GetArchivePath(options);
                string format = options.Format.ToLower();

                using var stream = File.Create(archivePath);

                IWriter writer = format switch
                {
                    "zip" => WriterFactory.Open(stream, ArchiveType.Zip, new WriterOptions(CompressionType.Deflate)
                    {
                        LeaveStreamOpen = false,
                        ArchiveEncoding = new ArchiveEncoding { Default = System.Text.Encoding.UTF8 }
                    }),

                    "tar" => WriterFactory.Open(stream, ArchiveType.Tar, new WriterOptions(CompressionType.None)),

                    "tar.gz" => new GZipStream(stream, CompressionMode.Compress, false) is var gzipStream
                        ? WriterFactory.Open(gzipStream, ArchiveType.GZip, new WriterOptions(CompressionType.None))
                        : throw new InvalidOperationException("[X]Failed to initialize GZip stream"),
                    _ => throw new NotSupportedException($"Format tidakdidukung oleh SharpCompress: {format}")
                };

                foreach (var path in options.InputPaths)
                {
                    if (Directory.Exists(path))
                    {
                        var baseDir = Path.GetFileName(path);
                        writer.WriteAll(path, "*", SearchOption.AllDirectories);
                    }
                    else if (File.Exists(path))
                    {
                        writer.Write(Path.GetFileName(path), File.OpenRead(path), null);
                    }
                    else
                    {
                        Console.WriteLine($"[X] Path tidak ditemukan: {path}");
                    }
                }
                writer.Dispose();

                Console.WriteLine($"✅ Arsip{format} berhasil dibuat di: {archivePath}");
            }
            catch (Exception Err)
            {
                System.Console.WriteLine($"[X] Gagal membuat arsip karena: {Err.Message}");
            }
        }

        private static string GetArchivePath(ArchiveOptions options)
        {
            string ext = options.Format.ToLower() switch
            {
                "zip" => ".zip",
                "tar" => ".tar",
                "tar.gz" => ".tar.gz",
                _ => ""
            };

            return options.OutputPath.EndsWith(ext) ? options.OutputPath : options.OutputPath + ext;
        }
    } 
}
    
