using System;
using System.Collections.Generic;

namespace BluesZencore.Core
{
    public static class ExtractorManager
    {
        private static readonly Dictionary<string, IArchiveExtractor> _extractors = new();
        public static IReadOnlyDictionary<string, IArchiveExtractor> Extractors => _extractors;
        static ExtractorManager()
        {
            // Daftarkan semua extractor yang tersedia
            _extractors["zip"] = new SharpExtractor();
            _extractors["7z"] = new ExternalExtractor("assets/7z.exe", "7z");
            _extractors["rar"] = new ExternalExtractor("assets/unrar.exe", "rar");
            
        }

        public static IArchiveExtractor GetExtractorForFormat(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
                throw new ArgumentException("Format tidak boleh kosong.");

            format = format.ToLowerInvariant();

            if (_extractors.TryGetValue(format, out var extractor))
                return extractor;

            throw new NotSupportedException($"Ekstraktor untuk format '{format}' tidak tersedia.");
        }
    }
}
