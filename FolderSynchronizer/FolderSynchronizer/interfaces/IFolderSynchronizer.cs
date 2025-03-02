namespace FolderSynchronizer.interfaces
{
    public interface IFolderSynchronizerService
    {
        void SynchronizeFolders(string source, string destination);
    }
}
