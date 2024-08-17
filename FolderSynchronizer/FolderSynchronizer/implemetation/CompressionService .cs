using FolderSynchronizer.interfaces;
using Microsoft.Extensions.Logging;
using System.IO.Compression;

namespace FolderSynchronizer.implemetation
{
    public class CompressionService : ICompressionService
    {
        private readonly ILogger<CompressionService> _logger;
        public CompressionService(ILogger<CompressionService> logger)
        {
            _logger = logger;
        }

        public byte[] Compress(byte[] data)
        {
            _logger.LogInformation("Compressing data");
            using var compressedStream = new MemoryStream();
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                zipStream.Write(data, 0, data.Length);
            }
            return compressedStream.ToArray();
        }
    }

}
