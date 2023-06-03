using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SupernoteDesktopClient.Messages
{
    public class SettingsChangedMessage : ValueChangedMessage<string>
    {
        public SettingsChangedMessage(string value) : base(value) { }
    }
}
