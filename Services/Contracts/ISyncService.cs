namespace SupernoteDesktopClient.Services.Contracts
{
    public interface ISyncService
    {
        bool IsBusy { get; }

        public string SourceLocation { get; }

        public string BackupLocation { get; }

        public string ArchiveLocation { get; }

        bool Sync();
    }
}