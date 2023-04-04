using MediaDevices;
using SupernoteDesktopClient.Services.Contracts;
using System.Diagnostics;
using System.Linq;

namespace SupernoteDesktopClient.Services
{
    public class MediaDeviceService : IMediaDeviceService
    {
        private const string _supernoteDeviceId = "VID_2207&PID_0011";

        public MediaDevice Device { get; set; }
        public MediaDriveInfo DriveInfo { get; set; }

        public MediaDeviceService()
        {
            UpdateMediaDeviceInfo();
        }

        public void UpdateMediaDeviceInfo()
        {
            if (Device?.IsConnected == true)
                Device.Disconnect();

            // if running under visual studio, do not select specific device
            if (Debugger.IsAttached == true)
                Device = MediaDevice.GetDevices().FirstOrDefault();
            else
                Device = MediaDevice.GetDevices().Where(p => p.DeviceId.Contains(_supernoteDeviceId, System.StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            DriveInfo = null;
            if (Device?.IsConnected == false)
            {
                Device.Connect();
                DriveInfo = Device?.GetDrives().FirstOrDefault();
            }
        }
    }
}
