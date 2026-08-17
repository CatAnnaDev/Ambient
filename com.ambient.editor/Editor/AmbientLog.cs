using UnityEngine;

namespace Ambient
{
    static class AmbientLog
    {
        public static void Say(string message, LogType type = LogType.Log)
        {
            message = "<b><color=#c81414>" + message + "</color></b>";
            var previous = Application.GetStackTraceLogType(type);
            Application.SetStackTraceLogType(type, StackTraceLogType.None);
            switch (type)
            {
                case LogType.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogType.Error:
                    Debug.LogError(message);
                    break;
                default:
                    Debug.Log(message);
                    break;
            }
            Application.SetStackTraceLogType(type, previous);
        }
    }
}
