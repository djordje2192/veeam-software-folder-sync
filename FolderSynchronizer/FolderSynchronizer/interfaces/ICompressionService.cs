namespace FolderSynchronizer.interfaces
{
    public interface ICompressionService
    {
        byte[] Compress(FileStream data);
    }
}
