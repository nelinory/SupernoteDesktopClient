# Supernote Desktop Client
![GitHub release](https://img.shields.io/github/v/release/nelinory/SupernoteDesktopClient)
![GitHub downloads](https://img.shields.io/github/downloads/nelinory/SupernoteDesktopClient/total)
![GitHub issues](https://img.shields.io/github/issues/nelinory/SupernoteDesktopClient)
![Github license](https://img.shields.io/github/license/nelinory/SupernoteDesktopClient)

Supernote Desktop Client (SDC) is a desktop client for Supernote paper-like tablet by Ratta (https://supernote.com).

> SDC is a windows application build with .NET 6.

### Key Features
- Automatically detects Supernote device connected with an USB cable
- Shows basic information for connected Supernote device
- Automatic/Manual Supernote storage synchronization to local folder
- Automatically archives last synchronization, have archives retention policy
- Supports multiple Supernote devices, each device have an unique local sync folder
- Build-in offline mode, which allows most of the application features to be used without having Supernote device connected
- Build-in explorer allows to view and open all Supernote files in the local backup folder (support for `*.note` & `*.mark` is coming soon)
- Automatic/Manual check for new application version
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

<table>
<tr>
	<td align="center"><b>Sync</b></td><td align="center"><b>Explorer</b></td>
</tr>
<tr>
	<td><img src="_Screenshots\sdc_sync.png" alt="sync"/></td>
	<td><img src="_Screenshots\sdc_explorer.png" alt="sync"/></td>
</tr>
<tr>
	<td align="center"><b>Settings</b></td><td align="center"><b>Themes<b></td>
</tr>
<tr>
	<td><img src="_Screenshots\sdc_settings.png" alt="sync"/></td>
	<td><img src="_Screenshots\sdc_themes.png" alt="sync"/></td>
</tr>
</table>
