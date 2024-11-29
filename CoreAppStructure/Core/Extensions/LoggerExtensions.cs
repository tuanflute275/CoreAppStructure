namespace CoreAppStructure.Core.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogWithTime(this MicrosoftLog.ILogger logger, string message, MicrosoftLog.LogLevel level = MicrosoftLog.LogLevel.Information)
            => logger.Log(level, $"{DateTime.Now:dd/MM/yyyy HH:mm:ss.fff}: {message}");
    }
}
