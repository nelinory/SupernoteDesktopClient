using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SupernoteDesktopClient.Messages
{
    public class SettingsChangedMessage : ValueChangedMessage<string>
    {
        public const string MINIMIZE_TO_TRAY_ENABLED = "MinimizeToTrayEnabled";

        public SettingsChangedMessage(string value) : base(value) { }
    }
}
