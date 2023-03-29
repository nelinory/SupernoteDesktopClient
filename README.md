# Supernote Desktop Client
Supernote Desktop Client (SDC) is a desktop client for Supernote paper-like tablet by Ratta.

### Features - battle plan
- [X] Build the application shell (.NET 6/WPF/MVVM)
- [X] Research how Windows mounts Supernote internal storage when connected with an USB cable - appears to use Media Transfer Protocol 1.0 (Android OS)
  - [X] ~~Build device detection using Windows Management Instrumentation (WMI)~~ - FAIL due to MTP protocol
  - [X] Build USB device insert/remove detection - SUCCESS
  - [ ] Add/Build MTP support for connected Supernote device
- [ ] Build dashboard to show basic information for connected Supernote device
- [ ] Build a sync functionality
  - [ ] Maintain sync backups
  - [ ] Sync everything from the Supernote device to a local folder (mirror mode)
- [ ] Research Supernote .note file format and build a note to image converter (use https://github.com/jya-dev/supernote-tool as guide)
- [ ] Integrate the .note converter into the client either as manual process or run in parallel to the main sync
- [ ] Provide a file explorer for the following Supernote folders:
  - [ ] Document
  - [ ] Export 
  - [ ] MyStyle
  - [ ] Note
  - [ ] Screenshot
- [ ] More to come ;) 
- [ ] Cleanup the github website and attach few screenshots
      