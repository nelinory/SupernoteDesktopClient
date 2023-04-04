using MediaDevices;

namespace SupernoteDesktopClient.Services.Contracts
{
    public interface IMediaDeviceService
    {
        MediaDevice Device { get; set; }
        
        MediaDriveInfo DriveInfo { get; set; }

        void UpdateMediaDeviceInfo();
    }
}