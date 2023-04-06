using MediaDevices;

namespace SupernoteDesktopClient.Services.Contracts
{
    public interface IMediaDeviceService
    {
        MediaDevice Device { get; }

        MediaDriveInfo DriveInfo { get; }

        void RefreshMediaDeviceInfo();
    }
}