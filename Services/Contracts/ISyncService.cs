namespace SupernoteDesktopClient.Services.Contracts
{
    public interface ISyncService
    {
        bool IsBusy { get; }

        bool Sync(string sourceFolder, string targetFolder, string deviceId);
    }
}