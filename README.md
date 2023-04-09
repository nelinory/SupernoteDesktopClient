# Supernote Desktop Client
Supernote Desktop Client (SDC) is a desktop client for Supernote paper-like tablet by Ratta (https://supernote.com).

> SDC is a windows application build with .NET 6

### Prototype - Features
- [X] Automatically detects Supernote device
- [X] Shows basic information for connected Supernote device
- [X] Manual Supernote storage synchronization to local folder
- [X] Build in backup support for up to 5 automatic backups
- [X] Supports multiple Supernote devices, each device will have unique local sync folder

### Release Stage Goals
- [ ] Research Supernote .note file format and build a note to image converter (use https://github.com/jya-dev/supernote-tool as guide)
- [ ] Extend the sync functionality
  - [ ] Sync everything from the Supernote device to a local folder (mirror mode)
  - [ ] Ability to automatic sync when Supernote device is connected
- [ ] Tray icon/menu support
- [ ] Integrate the .note converter into the client either as manual process or run in parallel with the main sync
- [ ] Provide a file explorer for the following Supernote folders:
  - [ ] Document
  - [ ] Export 
  - [ ] MyStyle
  - [ ] Note
  - [ ] Screenshot

### Problems with Microsoft .NET

If you do not have .NET 6 installed:
1. Go to https://dotnet.microsoft.com/en-us/download/dotnet/6.0
2. Find the section ".NET Desktop Runtime 6.x.x"
3. Download x64 Windows installer
4. Run the installer

After following these steps, you can open Terminal and type: `dotnet --info`. In the output look for section `.NET runtimes installed`, in this section you should see something like:

`Microsoft.NETCore.App 6.x.x [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]`

and

`Microsoft.WindowsDesktop.App 6.x.x [C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App]`  

as long as both version say 6.x.x you are good to go.
  
### Screenshots
##### Dashboard
<img src="_Screenshots\sdc_dashboard.png" alt="dashboard" width="900"/>

##### Manual Sync
<img src="_Screenshots\sdc_sync.png" alt="dashboard" width="900"/>