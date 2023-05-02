using Serilog.Context;

namespace SupernoteDesktopClient.Core
{
    static class DiagnosticLogger
    {
        public static void Log(string messageTemplate, params object[] args)
        {
            if (SettingsManager.Instance.Settings.General.DiagnosticLogEnabled == true)
            {
                using (LogContext.PushProperty("IsDiag", 1))
                {
                    Serilog.Log.Information(messageTemplate, args);
                }
            }
        }
    }
}
