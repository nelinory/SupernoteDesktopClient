# Supernote Desktop Client
![GitHub relase](https://img.shields.io/github/v/release/nelinory/SupernoteDesktopClient)
![GitHub dowloads](https://img.shields.io/github/downloads/nelinory/SupernoteDesktopClient/total)
![GitHub issues](https://img.shields.io/github/issues/nelinory/SupernoteDesktopClient)
![Github license](https://img.shields.io/github/license/nelinory/SupernoteDesktopClient)

Supernote Desktop Client (SDC) is a desktop client for Supernote paper-like tablet by Ratta (https://supernote.com).

> SDC is a windows application build with .NET 6. Tested with Windows 10 

### Key Features
- Automatically detects Supernote device connected with an USB cable
- Shows basic information for connected Supernote device
- Automatic/Manual Supernote storage synchronization to local folder
- Automatically archives last synchronization
- Supports multiple Supernote devices, each device will have an unique local sync folder
- Light/Dark Theme support 

### Tested on Windows version
- Windows 10 version 22H2 (OS Build 19045.2846)
- Windows 11 version 22H2 (OS Build 22621.1413)

### Download & Run
Get the latest portable version from [Releases page](https://github.com/nelinory/SupernoteDesktopClient/releases/latest).
Extract the zip file to a desired location.  
Run `SupernoteDesktopClient.exe` from inside sdc folder. 
> If you get a [SmartScreen](https://user-images.githubusercontent.com/25006819/115977864-555d4300-a5ae-11eb-948b-c0139f606a2d.png) popup, click on "More info" and then "Run anyway".
> The reason this popup appears is because the application is portable and it is not signed for distribution through Microsoft store..

### Wiki
- [Frequently asked questions](https://github.com/nelinory/SupernoteDesktopClient/wiki/FAQ)  
- [Problems with Microsoft .NET when running the application](https://github.com/nelinory/SupernoteDesktopClient/wiki/Problems-with-Microsoft-.NET-when-running-the-application)

### Roadmap
For release milestones, please check the project board: https://github.com/users/nelinory/projects/1

### How to build
- Clone SDC repository
- Open SupernoteDesktopClient solution in Visual Studio 2022 and build it

### Screenshots
##### Dashboard
<img src="_Screenshots\sdc_dashboard.png" alt="dashboard" width="900"/>

##### Manual Sync
<img src="_Screenshots\sdc_sync.png" alt="dashboard" width="900"/>