using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SupernoteDesktopClient.Messages
{
    public class ProgressTrackActionMessage : ValueChangedMessage<bool>
    {
        public ProgressTrackActionMessage(bool value) : base(value) { }
    }
}
