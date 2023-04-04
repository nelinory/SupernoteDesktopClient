using CommunityToolkit.Mvvm.Messaging.Messages;
using MediaDevices;

namespace SupernoteDesktopClient.Models
{
    public class MediaDeviceChangedMessage : ValueChangedMessage<MediaDevice>
    {
        public MediaDeviceChangedMessage(MediaDevice mediaDevice) : base(mediaDevice) { }
    }
}
