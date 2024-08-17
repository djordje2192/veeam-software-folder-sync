namespace FolderSynchronizer.interfaces
{
    public interface IEncryptionService
    {
        byte[] Encrypt(byte[] data, string key);
    }
}
