using System;
using UnityEditor;

namespace Ambient
{
    [InitializeOnLoad]
    static class Escalation
    {
        const string KFirst = "ambient.first_seen";
        const string KOpens = "ambient.open_count";
        const string SKSession = "ambient.session_flag";
        const string SKStart = "ambient.session_start";

        static Escalation()
        {
            if (!EditorPrefs.HasKey(KFirst))
            {
                EditorPrefs.SetString(KFirst, DateTime.Now.Ticks.ToString());
            }
            if (!SessionState.GetBool(SKSession, false))
            {
                SessionState.SetBool(SKSession, true);
                SessionState.SetString(SKStart, DateTime.Now.Ticks.ToString());
                EditorPrefs.SetInt(KOpens, EditorPrefs.GetInt(KOpens, 0) + 1);
            }
        }

        public static int Opens => EditorPrefs.GetInt(KOpens, 1);

        public static int Days
        {
            get
            {
                try
                {
                    var first = new DateTime(long.Parse(EditorPrefs.GetString(KFirst, DateTime.Now.Ticks.ToString())));
                    int d = (int)(DateTime.Now - first).TotalDays;
                    return d < 0 ? 0 : d;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public static double SessionMinutes
        {
            get
            {
                try
                {
                    var start = new DateTime(long.Parse(SessionState.GetString(SKStart, DateTime.Now.Ticks.ToString())));
                    return (DateTime.Now - start).TotalMinutes;
                }
                catch
                {
                    return 0.0;
                }
            }
        }

        public static float Intensity
        {
            get
            {
                double v = 0.15 + (SessionMinutes / 90.0) * 0.5 + Days * 0.06 + (Opens > 1 ? 0.05 : 0.0);
                return (float)(v < 0.0 ? 0.0 : (v > 1.0 ? 1.0 : v));
            }
        }
    }
}
