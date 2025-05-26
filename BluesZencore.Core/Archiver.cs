using System;
using System.Xml;
using System.Collections.Generic;



namespace BluesZencore.Core
{
    public static class Archiver
    {
        public static void CreateArchive(ArchiveOptions options)
        {
            Console.WriteLine($"Membuat arsip dalam format: {options.Format}");
            bool useCli = options.AdvancedMode;
            if (!string.IsNullOrWhiteSpace(options.archiveNameTemplate))
            {
                string filename = TemplateResolver.ResolveTemplate(options.archiveNameTemplate);
                string? dir = Path.GetDirectoryName(options.OutputPath);
                string ext = options.Format;

                options.OutputPath = Path.Combine(dir ?? ".", Path.ChangeExtension(filename, ext));
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