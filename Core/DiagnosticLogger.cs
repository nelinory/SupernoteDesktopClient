using Serilog.Context;
using System.Runtime.CompilerServices;

namespace SupernoteDesktopClient.Core
{
    static class DiagnosticLogger
    {
        public static void Log(string messageTemplate, [CallerMemberName] string callerName = "", params object[] args)
        {
            if (SettingsManager.Instance.Settings.General.DiagnosticLogEnabled == true)
            {
                using (LogContext.PushProperty("IsDiag", 1))
                {
                    Serilog.Log.Information($"Caller: {callerName} - " + messageTemplate, args);
                }
            }
        }
    }
}
