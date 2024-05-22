using Gpm.Common.Log;
using System;
using System.Runtime.CompilerServices;

namespace Gpm.Adapter.Internal
{
    public static class LoggerMapper
    {
        public static void Debug(string message, Type classType, [CallerMemberName] string methodName = "")
        {
            GpmLogger.Debug(message, GpmAdapter.SERVICE_NAME, classType, methodName);
        }

        public static void Warn(string message, Type classType, [CallerMemberName] string methodName = "")
        {
            GpmLogger.Warn(message, GpmAdapter.SERVICE_NAME, classType, methodName);
        }

        public static void Error(string message, Type classType, [CallerMemberName] string methodName = "")
        {
            GpmLogger.Error(message, GpmAdapter.SERVICE_NAME, classType, methodName);
        }
    }
}