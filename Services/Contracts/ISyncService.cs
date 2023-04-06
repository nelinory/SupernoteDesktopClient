namespace SupernoteDesktopClient.Services.Contracts
{
    public interface ISyncService
    {
        bool Sync(string sourceFolder, string targetFolder);
    }
}