using MediaDevices;
using SupernoteDesktopClient.Core;
using SupernoteDesktopClient.Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace SupernoteDesktopClient.Services
{
    public class MediaDeviceService : IMediaDeviceService
    {
        private const string _supernoteDeviceId = "VID_2207&PID_0011";

        private MediaDevice _device;
        public MediaDevice Device
        {
            get { return _device; }
        }

        private MediaDriveInfo _driveInfo;
        public MediaDriveInfo DriveInfo
        {
            get { return _driveInfo; }
        }

        public MediaDeviceService()
        {
            RefreshMediaDeviceInfo();
        }

        public void RefreshMediaDeviceInfo()
        {
            List<MediaDevice> tmpDevices = MediaDevice.GetDevices().ToList();
            MediaDevice tmpDevice = tmpDevices.Where(p => p.DeviceId.Contains(_supernoteDeviceId, System.StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (_device == null)
                _device = tmpDevice;
            else
            {
                if (_device != tmpDevice && _device.IsConnected == true)
                    _device.Disconnect();

                _device = tmpDevice;
            }

            if (_device != null && _device.IsConnected == false)
                _device.Connect();

            _driveInfo = null;
            if (_device != null && _device.IsConnected == true)
                _driveInfo = _device.GetDrives().FirstOrDefault();

            DiagnosticLogger.Log($"Device: {(_device == null ? "N/A":_device)}, DriveInfo: {(_driveInfo == null ? "N/A" : _driveInfo)}");
        }
    }
}
