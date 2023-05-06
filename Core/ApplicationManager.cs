using System;

namespace SupernoteDesktopClient.Core
{
    public static class ApplicationManager
    {
        public static string GetAssemblyVersion()
        {
            Version versionObject = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;

            return $"v{versionObject.Major}.{versionObject.Minor}.{versionObject.Build}";
        }
    }
}
