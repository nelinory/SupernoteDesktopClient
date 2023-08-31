using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SupernoteDesktopClient.Messages
{
    public class ConversionFailedMessage : ValueChangedMessage<string>
    {
        public ConversionFailedMessage(string value) : base(value) { }
    }
}