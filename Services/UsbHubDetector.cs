using SupernoteDesktopClient.Core;
using SupernoteDesktopClient.Services.Contracts;
using System;
using System.ComponentModel;
using System.Management;
using System.Threading;

namespace SupernoteDesktopClient.Services
{
    public delegate void UsbHubStateChangedEventHandler(string deviceId, bool isConnected);

    public class UsbHubDetector : IUsbHubDetector, IDisposable
    {
        private const string _wmiQuery = "SELECT * FROM {0} WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'";
        private const string _creationEvent = "__InstanceCreationEvent";
        private const string _deletionEvent = "__InstanceDeletionEvent";

        private readonly ManagementEventWatcher _insertManagementEventWatcher = new ManagementEventWatcher(new WqlEventQuery(String.Format(_wmiQuery, _creationEvent)));
        private readonly ManagementEventWatcher _removeManagementEventWatcher = new ManagementEventWatcher(new WqlEventQuery(String.Format(_wmiQuery, _deletionEvent)));
        private bool disposedValue;

        public UsbHubDetector()
        {
            BackgroundWorker bwDriveDetector = new BackgroundWorker();
            bwDriveDetector.DoWork += DoWork;
            bwDriveDetector.RunWorkerAsync();
        }

        public event UsbHubStateChangedEventHandler UsbHubStateChanged;

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue == false)
            {
                if (disposing == true)
                {
                    _insertManagementEventWatcher.Stop();
                    _removeManagementEventWatcher.Stop();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            _insertManagementEventWatcher.EventArrived += new EventArrivedEventHandler(MapEventArgs);
            _insertManagementEventWatcher.Start();

            _removeManagementEventWatcher.EventArrived += new EventArrivedEventHandler(MapEventArgs);
            _removeManagementEventWatcher.Start();
        }

        private void MapEventArgs(object sender, EventArrivedEventArgs e)
        {
            string deviceId = ((ManagementBaseObject)e.NewEvent["TargetInstance"])["PNPDeviceID"].ToString();
            bool isConnected = (e.NewEvent.ClassPath.RelativePath == _creationEvent);

            DiagnosticLogger.Log($"Usb Device: {deviceId}, IsConnected: {isConnected}");

            // WMI device event seems to be triggering immediately when device is attached/detached.
            // Adding 1s wait time before notifying all subscribers to ensure the device is available for them.
            Thread.Sleep(1000);

            UsbHubStateChanged?.Invoke(deviceId, isConnected);
        }
    }
}
