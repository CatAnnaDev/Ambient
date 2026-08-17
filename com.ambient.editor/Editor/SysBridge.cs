using System;
using System.Runtime.InteropServices;
using UnityEditor;

namespace Ambient
{
    [InitializeOnLoad]
    public static class SysBridge
    {
        const string Lib = "AmbientSys";

        [DllImport(Lib)] static extern int ambient_sys_ping();
        [DllImport(Lib)] static extern int ambient_hour();
        [DllImport(Lib)] static extern double ambient_uptime_hours();
        [DllImport(Lib)] static extern int ambient_battery();
        [DllImport(Lib)] static extern IntPtr ambient_username();

        public static bool Available { get; private set; }

        static SysBridge()
        {
            try
            {
                Available = ambient_sys_ping() == 7;
            }
            catch
            {
                Available = false;
            }
        }

        public static int Hour()
        {
            try { return Available ? ambient_hour() : -1; }
            catch { return -1; }
        }

        public static double UptimeHours()
        {
            try { return Available ? ambient_uptime_hours() : -1.0; }
            catch { return -1.0; }
        }

        public static int Battery()
        {
            try { return Available ? ambient_battery() : -1; }
            catch { return -1; }
        }

        public static string User()
        {
            try
            {
                if (!Available) return null;
                var p = ambient_username();
                return p == IntPtr.Zero ? null : Marshal.PtrToStringAnsi(p);
            }
            catch { return null; }
        }
    }
}
