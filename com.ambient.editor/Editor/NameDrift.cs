using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Ambient
{
    [InitializeOnLoad]
    static class NameDrift
    {
        static readonly char[] Marks =
        {
            '\u0300', '\u0301', '\u0302', '\u0303', '\u0308',
            '\u030A', '\u0323', '\u0324', '\u0327', '\u0489',
        };

        static double nextTick;

        static NameDrift()
        {
            EditorApplication.hierarchyWindowItemByEntityIdOnGUI += OnHierarchy;
            EditorApplication.projectWindowItemOnGUI += OnProject;
            EditorApplication.update += Tick;
        }

        static void Tick()
        {
            if (!AmbientMenu.IsOn(AmbientMenu.Names))
            {
                return;
            }
            double t = EditorApplication.timeSinceStartup;
            if (t >= nextTick)
            {
                nextTick = t + (Director.Surging ? 0.1 : 1.6);
                EditorApplication.RepaintHierarchyWindow();
                EditorApplication.RepaintProjectWindow();
            }
        }

        static int Phase() => (int)(EditorApplication.timeSinceStartup / 1.6);

        static void OnHierarchy(EntityId id, Rect rect)
        {
            if (!AmbientMenu.IsOn(AmbientMenu.Names) || Event.current.type != EventType.Repaint)
            {
                return;
            }
            var go = EditorUtility.EntityIdToObject(id) as GameObject;
            if (go == null)
            {
                return;
            }
            if (Selection.Contains(go))
            {
                return;
            }
            string corrupted = Corrupt(go.name, id.GetHashCode(), Phase());
            if (corrupted != null)
            {
                DrawOver(rect, corrupted);
            }
        }

        static void OnProject(string guid, Rect rect)
        {
            if (!AmbientMenu.IsOn(AmbientMenu.Names) || Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (rect.height > 20f)
            {
                return;
            }
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            string corrupted = Corrupt(name, guid.GetHashCode(), Phase());
            if (corrupted != null)
            {
                DrawOver(rect, corrupted);
            }
        }

        static void DrawOver(Rect rect, string text)
        {
            var label = rect;
            label.xMin += 18f;
            Color bg = EditorGUIUtility.isProSkin
                ? new Color(0.219f, 0.219f, 0.219f, 1f)
                : new Color(0.784f, 0.784f, 0.784f, 1f);
            EditorGUI.DrawRect(label, bg);
            GUI.Label(label, text, EditorStyles.label);
        }

        static string Corrupt(string name, int seed, int phase)
        {
            var rng = new System.Random(unchecked(seed * 397) ^ phase);
            double d = rng.NextDouble();
            float env = Director.Envelope() * Director.SurgeStrength;
            double keep = 0.85 - env * 0.75;
            if (d < keep)
            {
                return null;
            }
            if (rng.NextDouble() < 0.6)
            {
                return Cryptic.Names[rng.Next(Cryptic.Names.Length)];
            }
            return Zalgo(name, rng);
        }

        static string Zalgo(string source, System.Random rng)
        {
            var sb = new StringBuilder(source.Length * 3);
            foreach (char c in source)
            {
                sb.Append(c);
                int n = rng.Next(0, 3);
                for (int i = 0; i < n; i++)
                {
                    sb.Append(Marks[rng.Next(Marks.Length)]);
                }
            }
            return sb.ToString();
        }
    }
}
