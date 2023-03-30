namespace SupernoteDesktopClient.Services
{
    public interface IUsbHubDetector
    {
        event UsbHubStateChangedEventHandler UsbHubStateChanged;
    }
}