using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ambient
{
    [InitializeOnLoad]
    static class Quake
    {
        static readonly char[] Marks =
        {
            '\u0300', '\u0301', '\u0302', '\u0303', '\u0308', '\u030A', '\u0323', '\u0489',
        };

        static readonly Dictionary<EditorWindow, string> titles = new Dictionary<EditorWindow, string>();
        static bool active;
        static double nextTitle;

        static Quake()
        {
            Director.OnSurge += OnSurge;
            EditorApplication.update += Tick;
        }

        static void OnSurge()
        {
            if (!AmbientMenu.IsOn(AmbientMenu.HauntEditor) || active)
            {
                return;
            }
            if (!Director.IsTakeover && Director.SurgeStrength < 0.45f)
            {
                return;
            }
            titles.Clear();
            foreach (var w in Resources.FindObjectsOfTypeAll<EditorWindow>())
            {
                if (w == null || w is AmbientPreview)
                {
                    continue;
                }
                var r = w.rootVisualElement;
                if (r == null || r.panel == null)
                {
                    continue;
                }
                string orig = w.titleContent != null ? w.titleContent.text : null;
                titles[w] = orig;
                if (w.titleContent != null && !string.IsNullOrEmpty(orig))
                {
                    w.titleContent.text = Zalgo(orig);
                }
            }
            active = titles.Count > 0;
        }

        static void Tick()
        {
            if (!active)
            {
                return;
            }
            if (!Director.Surging)
            {
                End();
                return;
            }
            float env = Director.Envelope() * Director.SurgeStrength;
            float amp = 4.5f * env;
            double t = EditorApplication.timeSinceStartup;
            var current = EditorHaunt.Current;

            int i = 0;
            foreach (var kv in titles)
            {
                var w = kv.Key;
                i++;
                if (w == null || w == current || PanelPhysics.IsFalling(w))
                {
                    continue;
                }
                var r = w.rootVisualElement;
                if (r == null || r.panel == null)
                {
                    continue;
                }
                float dx = Mathf.Sin((float)t * 42f + i) * amp;
                float dy = Mathf.Cos((float)t * 33f + i) * amp * 0.6f;
                r.style.translate = new StyleTranslate(new Translate(new Length(dx), new Length(dy)));
            }

            if (t >= nextTitle)
            {
                nextTitle = t + 0.14;
                foreach (var kv in titles)
                {
                    var w = kv.Key;
                    if (w != null && w.titleContent != null && !string.IsNullOrEmpty(kv.Value))
                    {
                        w.titleContent.text = Zalgo(kv.Value);
                    }
                }
            }
        }

        static void End()
        {
            foreach (var kv in titles)
            {
                var w = kv.Key;
                if (w == null)
                {
                    continue;
                }
                var r = w.rootVisualElement;
                if (r != null)
                {
                    try
                    {
                        r.style.translate = new StyleTranslate(new Translate(new Length(0f), new Length(0f)));
                    }
                    catch
                    {
                    }
                }
                if (w.titleContent != null && kv.Value != null)
                {
                    w.titleContent.text = kv.Value;
                }
            }
            titles.Clear();
            active = false;
        }

        static string Zalgo(string s)
        {
            var sb = new StringBuilder(s.Length * 3);
            foreach (char c in s)
            {
                sb.Append(c);
                int n = UnityEngine.Random.Range(0, 3);
                for (int i = 0; i < n; i++)
                {
                    sb.Append(Marks[UnityEngine.Random.Range(0, Marks.Length)]);
                }
            }
            return sb.ToString();
        }
    }
}
