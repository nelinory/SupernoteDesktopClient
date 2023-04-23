# Supernote Desktop Client
![GitHub relase](https://img.shields.io/github/v/release/nelinory/SupernoteDesktopClient)
![GitHub dowloads](https://img.shields.io/github/downloads/nelinory/SupernoteDesktopClient/total)
![GitHub issues](https://img.shields.io/github/issues/nelinory/SupernoteDesktopClient)
![Github license](https://img.shields.io/github/license/nelinory/SupernoteDesktopClient)

Supernote Desktop Client (SDC) is a desktop client for Supernote paper-like tablet by Ratta (https://supernote.com).

> SDC is a windows application build with .NET 6

### Key Features
- Automatically detects Supernote device connected with an USB cable
- Shows basic information for connected Supernote device
- Automatic/Manual Supernote storage synchronization to local folder
- Automatically archives last synchronization
- Supports multiple Supernote devices, each device will have unique local sync folder
- Light/Dark Theme support 

### Download
Get the latest portable version from [Releases page](https://github.com/nelinory/SupernoteDesktopClient/releases/latest).
Extract the zip file to a desired location.  
Run `SupernoteDesktopClient.exe` from inside sdc folder. 

### Problems with Microsoft .NET when running the application

If you do not have .NET 6 installed:
1. Go to https://dotnet.microsoft.com/en-us/download/dotnet/6.0
2. Find the section ".NET Desktop Runtime 6.x.x"
3. Download x64 Windows installer
4. Run the installer

After following these steps, you can open Terminal and type: `dotnet --info`. In the output look for section `.NET runtimes installed`, in this section you should see something like:  
`Microsoft.NETCore.App 6.x.x [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]`  
and  
`Microsoft.WindowsDesktop.App 6.x.x [C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App]`  
as long as both versions say 6.x.x you are good to go.

### Roadmap
For release milestones, please check the project board: https://github.com/users/nelinory/projects/1

### How to build

- Visual Studio 2022
- .NET 6.0 Runtime
- .NET 6.0 SDK

### Screenshots
##### Dashboard
<img src="_Screenshots\sdc_dashboard.png" alt="dashboard" width="900"/>

##### Manual Sync
<img src="_Screenshots\sdc_sync.png" alt="dashboard" width="900"/>