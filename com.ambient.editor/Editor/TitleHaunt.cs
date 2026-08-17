using System;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Ambient
{
    [InitializeOnLoad]
    static class TitleHaunt
    {
        static readonly char[] Marks =
        {
            '\u0300', '\u0301', '\u0302', '\u0303', '\u0308', '\u030A', '\u0323', '\u0489',
        };

        static MethodInfo refresh;
        static FieldInfo titleField;
        static PropertyInfo titleProp;
        static bool glitching;
        static string current;
        static double nextRefresh;

        static TitleHaunt()
        {
            try
            {
                var ed = typeof(EditorApplication);
                var evt = ed.GetEvent("updateMainWindowTitle", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (evt != null)
                {
                    var handler = Delegate.CreateDelegate(evt.EventHandlerType, typeof(TitleHaunt).GetMethod("OnTitle", BindingFlags.Static | BindingFlags.NonPublic));
                    evt.AddEventHandler(null, handler);
                }
                refresh = FindRefresh(ed);
                Director.OnSurge += OnSurge;
                EditorApplication.update += Tick;
            }
            catch
            {
            }
        }

        static MethodInfo FindRefresh(Type ed)
        {
            string[] names = { "UpdateMainWindowTitle", "CallUpdateMainWindowTitle", "RequestRepaintOfAllEditorsAndGameViews" };
            foreach (var n in names)
            {
                var m = ed.GetMethod(n, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                if (m != null)
                {
                    return m;
                }
            }
            return null;
        }

        static void OnTitle(object desc)
        {
            if (!glitching || desc == null)
            {
                return;
            }
            try
            {
                if (titleField == null && titleProp == null)
                {
                    titleField = desc.GetType().GetField("title", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (titleField == null)
                    {
                        titleProp = desc.GetType().GetProperty("title", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    }
                }
                if (titleField != null)
                {
                    titleField.SetValue(desc, current);
                }
                else if (titleProp != null && titleProp.CanWrite)
                {
                    titleProp.SetValue(desc, current);
                }
            }
            catch
            {
            }
        }

        static void OnSurge()
        {
            if (!AmbientMenu.IsOn(AmbientMenu.HauntEditor))
            {
                return;
            }
            if (!Director.IsTakeover && Director.SurgeStrength < 0.4f)
            {
                return;
            }
            glitching = true;
            current = Zalgo(Director.SurgeMessage);
            Refresh();
        }

        static void Tick()
        {
            if (!glitching)
            {
                return;
            }
            double t = EditorApplication.timeSinceStartup;
            if (!Director.Surging)
            {
                glitching = false;
                Refresh();
                return;
            }
            if (t >= nextRefresh)
            {
                nextRefresh = t + 0.18;
                current = Zalgo(Director.SurgeMessage);
                Refresh();
            }
        }

        static void Refresh()
        {
            try
            {
                refresh?.Invoke(null, null);
            }
            catch
            {
            }
        }

        static string Zalgo(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                s = "signal lost";
            }
            var sb = new StringBuilder(s.Length * 4);
            foreach (char c in s)
            {
                sb.Append(c);
                int n = UnityEngine.Random.Range(1, 4);
                for (int i = 0; i < n; i++)
                {
                    sb.Append(Marks[UnityEngine.Random.Range(0, Marks.Length)]);
                }
            }
            return sb.ToString();
        }
    }
}
