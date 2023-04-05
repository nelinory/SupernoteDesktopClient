using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SupernoteDesktopClient.Models
{
    public class MediaDeviceChangedMessage : ValueChangedMessage<string>
    {
        public MediaDeviceChangedMessage(string deviceId) : base(deviceId) { }
    }
}
