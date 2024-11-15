﻿using MediaDevices;
using SupernoteDesktopClient.Core;
using SupernoteDesktopClient.Extensions;
using SupernoteDesktopClient.Models;
using SupernoteDesktopClient.Services.Contracts;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SupernoteDesktopClient.Services
{
    public class MediaDeviceService : IMediaDeviceService
    {
        private const string SUPERNOTE_DEVICE_ID = "VID_2207&PID_0011";
        private const string NOMAD_DEVICE_ID = "VID_2207&PID_0007";
        private const string SUPERNOTE_DESCRIPTION = "supernote";
        private const string NOMAD_DESCRIPTION = "nomad";

        private MediaDriveInfo _driveInfo;

        private bool _isDeviceConnected;
        public bool IsDeviceConnected
        {
            get { return _isDeviceConnected; }
        }

        private SupernoteInfo _supernoteInfo;
        public SupernoteInfo SupernoteInfo
        {
            get { return _supernoteInfo; }
        }

        private MediaDevice _supernoteManager;
        public MediaDevice SupernoteManager
        {
            get { return _supernoteManager; }
        }

        public MediaDeviceService()
        {
            RefreshMediaDeviceInfo();
        }

        public void RefreshMediaDeviceInfo()
        {
            MediaDevice tmpDevice = MediaDevice.GetDevices().Where(p => p.DeviceId.Contains(SUPERNOTE_DEVICE_ID, System.StringComparison.InvariantCultureIgnoreCase)
                                                                    || p.DeviceId.Contains(NOMAD_DEVICE_ID, System.StringComparison.InvariantCultureIgnoreCase)
                                                                    || p.Description.Contains(SUPERNOTE_DESCRIPTION, System.StringComparison.InvariantCultureIgnoreCase)
                                                                    || p.Description.Contains(NOMAD_DESCRIPTION, System.StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

#if DEBUG
            if (tmpDevice != null)
                Debug.WriteLine($"Usb Device: {tmpDevice?.DeviceId ?? "N/A"}, Description: {tmpDevice?.Description ?? "N/A"}");
#endif

            if (_supernoteManager == null)
                _supernoteManager = tmpDevice;
            else
            {
                if (_supernoteManager != tmpDevice && _supernoteManager.IsConnected == true)
                    _supernoteManager.Disconnect();

                _supernoteManager = tmpDevice;
            }

            if (_supernoteManager != null && _supernoteManager.IsConnected == false)
                _supernoteManager.Connect();

            _driveInfo = null;
            if (_supernoteManager != null && _supernoteManager.IsConnected == true)
                _driveInfo = _supernoteManager.GetDrives().FirstOrDefault();

            _isDeviceConnected = (_supernoteManager != null && _supernoteManager.IsConnected == true);

            // load supernoteInfo object
            if (_isDeviceConnected == true)
            {
                _supernoteInfo = new SupernoteInfo
                {
                    Model = _supernoteManager?.Model,
                    SerialNumber = _supernoteManager?.SerialNumber,
                    SerialNumberHash = _supernoteManager?.SerialNumber.GetShortSHA1Hash(),
                    PowerLevel = _supernoteManager?.PowerLevel ?? 0,
                    AvailableFreeSpace = _driveInfo?.AvailableFreeSpace ?? 0,
                    TotalSpace = _driveInfo?.TotalSize ?? 0,
                    RootFolder = _driveInfo?.RootDirectory.FullName
                };

                // store/update device profile
                if (SettingsManager.Instance.Settings.DeviceProfiles.ContainsKey(_supernoteInfo.SerialNumberHash) == true)
                    SettingsManager.Instance.Settings.DeviceProfiles[_supernoteInfo.SerialNumberHash] = _supernoteInfo;
                else
                    SettingsManager.Instance.Settings.DeviceProfiles.Add(_supernoteInfo.SerialNumberHash, _supernoteInfo);

                // clear active state
                foreach (KeyValuePair<string, SupernoteInfo> kvp in SettingsManager.Instance.Settings.DeviceProfiles)
                {
                    kvp.Value.Active = false;
                }

                // mark connected device as active
                SettingsManager.Instance.Settings.DeviceProfiles[_supernoteInfo.SerialNumberHash].Active = true;
            }
            else
            {
                _supernoteInfo = new SupernoteInfo();
                if (SettingsManager.Instance.Settings.DeviceProfiles.Count > 0)
                {
                    // select the first active profile
                    _supernoteInfo = SettingsManager.Instance.Settings.DeviceProfiles.Where(p => p.Value.Active == true).FirstOrDefault().Value;

                    // if no active profile found, select the first one and make it active
                    if (_supernoteInfo == null)
                    {
                        _supernoteInfo = SettingsManager.Instance.Settings.DeviceProfiles.FirstOrDefault().Value;
                        _supernoteInfo.Active = true;
                    }
                }
            }

            DiagnosticLogger.Log($"Usb Device: {_supernoteManager?.DeviceId ?? "N/A"}, " +
                                    $"Description: {_supernoteManager?.Description ?? "N/A"}, " +
                                    $"Model: {_supernoteManager?.Model ?? "N/A"}, " +
                                    $"DriveInfo: {_driveInfo?.TotalSize.GetDataSizeAsString() ?? "N/A"}");
        }
    }
}
