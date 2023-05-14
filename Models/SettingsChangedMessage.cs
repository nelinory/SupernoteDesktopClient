using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SupernoteDesktopClient.Models
{
    public class SettingsChangedMessage : ValueChangedMessage<string>
    {
        public SettingsChangedMessage(string value) : base(value) { }
    }
}
