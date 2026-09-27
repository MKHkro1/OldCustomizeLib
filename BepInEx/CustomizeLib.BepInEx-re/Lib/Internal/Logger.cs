using BepInEx.Logging;
using CustomizeLib.BepInEx.Internal.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal
{
    internal static class Logger
    {
        internal static void Log(LogLevel level) => LibData.Instance.Logger?.Log(level, "");
        internal static void Log(LogLevel level, object message) => LibData.Instance.Logger?.Log(level, message);

        internal static void LogInfo() => LibData.Instance.Logger?.LogInfo("");
        internal static void LogInfo(object messaege) => LibData.Instance.Logger?.LogInfo(messaege);

        internal static void LogWarning() => LibData.Instance.Logger?.LogWarning("");
        internal static void LogWarning(object message) => LibData.Instance.Logger?.LogWarning(message);

        internal static void LogError() => LibData.Instance.Logger?.LogError("");
        internal static void LogError(object message) => LibData.Instance.Logger?.LogError(message);

        internal static void LogDebug() => LibData.Instance.Logger?.LogDebug("");
        internal static void LogDebug(object message) => LibData.Instance.Logger?.LogDebug(message);

        internal static void LogMessage() => LibData.Instance.Logger?.LogMessage("");
        internal static void LogMessage(object messaege) => LibData.Instance.Logger?.LogMessage(messaege);

        internal static void LogFatal() => LibData.Instance.Logger?.LogFatal("");
        internal static void LogFatal(object message) => LibData.Instance.Logger?.LogFatal(message);
    }
}
