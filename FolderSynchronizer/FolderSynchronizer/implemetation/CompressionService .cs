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

        public byte[] Compress(FileStream data)
        {
            _logger.LogInformation("Compressing data");
            using var compressedStream = new MemoryStream();
            // Create the GZipStream on top of the compressedStream (destination)
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                // Copy the data from the input FileStream to the zipStream (which compresses it)
                data.CopyTo(zipStream);
            }
            return compressedStream.ToArray();
        }
    }

}
