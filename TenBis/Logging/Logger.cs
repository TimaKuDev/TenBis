using NLog;
using System.Runtime.CompilerServices;

namespace TenBis.Logging
{
    public static class Logger
    {
        private static readonly NLog.Logger m_Logger = LogManager.GetCurrentClassLogger();

        private static string FormatMessage(string message, string caller)
        {
            return $"{caller}: {message}";
        }

        public static void FunctionStarted([CallerMemberName] string caller = "")
        {
            m_Logger.Info(FormatMessage("Started execution", caller));
        }

        public static void FunctionFinished([CallerMemberName] string caller = "")
        {
            m_Logger.Info(FormatMessage("Finished execution", caller));
        }

        public static void Info(string message, [CallerMemberName] string caller = "") => 
            m_Logger.Info(FormatMessage(message, caller));

        public static void Info(string message, object[] args, [CallerMemberName] string caller = "") =>
            m_Logger.Info(FormatMessage(message, caller), args);

        public static void Info(Exception ex, string message, [CallerMemberName] string caller = "") =>
            m_Logger.Info(ex, FormatMessage(message, caller));

        public static void Warn(string message, [CallerMemberName] string caller = "") =>
            m_Logger.Warn(FormatMessage(message, caller));

        public static void Warn(string message, object[] args, [CallerMemberName] string caller = "") =>
            m_Logger.Warn(FormatMessage(message, caller), args);

        public static void Warn(Exception ex, string message, [CallerMemberName] string caller = "") =>
            m_Logger.Warn(ex, FormatMessage(message, caller));

        public static void Error(string message, [CallerMemberName] string caller = "") =>
            m_Logger.Error(FormatMessage(message, caller));

        public static void Error(string message, object[] args, [CallerMemberName] string caller = "") =>
            m_Logger.Error(FormatMessage(message, caller), args);

        public static void Error(Exception ex, string message, [CallerMemberName] string caller = "") =>
            m_Logger.Error(ex, FormatMessage(message, caller));

        public static void Debug(string message, [CallerMemberName] string caller = "") =>
            m_Logger.Debug(FormatMessage(message, caller));

        public static void Debug(string message, object[] args, [CallerMemberName] string caller = "") =>
            m_Logger.Debug(FormatMessage(message, caller), args);

        public static void Debug(Exception ex, string message, [CallerMemberName] string caller = "") =>
            m_Logger.Debug(ex, FormatMessage(message, caller));

        public static void Fatal(string message, [CallerMemberName] string caller = "") =>
            m_Logger.Fatal(FormatMessage(message, caller));

        public static void Fatal(string message, object[] args, [CallerMemberName] string caller = "") =>
            m_Logger.Fatal(FormatMessage(message, caller), args);

        public static void Fatal(Exception ex, string message, [CallerMemberName] string caller = "") =>
            m_Logger.Fatal(ex, FormatMessage(message, caller));

        public static void Log(LogLevel level, string message, [CallerMemberName] string caller = "") =>
            m_Logger.Log(level, FormatMessage(message, caller));

        public static void Log(LogLevel level, Exception ex, string message, [CallerMemberName] string caller = "") =>
            m_Logger.Log(level, ex, FormatMessage(message, caller));

        public static NLog.Logger GetLogger(string name) => LogManager.GetLogger(name);
    }
}
