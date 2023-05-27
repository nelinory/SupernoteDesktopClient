using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SupernoteDesktopClient.Messages
{
    public class MediaDeviceChangedMessage : ValueChangedMessage<DeviceInfo>
    {
        public MediaDeviceChangedMessage(DeviceInfo deviceInfo) : base(deviceInfo) { }
    }

    public class DeviceInfo
    {
        public string Deviceid { get; private set; }
        public bool IsConnected { get; private set; }

        public DeviceInfo(string deviceId, bool isConnected)
        {
            Deviceid = deviceId;
            IsConnected = isConnected;
        }
    }
}
