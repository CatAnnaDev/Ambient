using UnityEditor;
using UnityEngine;

namespace Ambient
{
    [InitializeOnLoad]
    static class WatchingObjects
    {
        static Texture2D sclera;
        static Texture2D iris;

        static WatchingObjects()
        {
            SceneView.duringSceneGui += OnScene;
        }

        static void Ensure()
        {
            if (sclera != null && iris != null)
            {
                return;
            }
            sclera = Disc(28, new Color(0.9f, 0.88f, 0.83f, 1f));
            iris = Disc(20, new Color(0.05f, 0.02f, 0.02f, 1f));
        }

        static Texture2D Disc(int size, Color fill)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.hideFlags = HideFlags.HideAndDontSave;
            float r = size * 0.5f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - r + 0.5f;
                    float dy = y - r + 0.5f;
                    float a = (dx * dx + dy * dy) <= r * r ? 1f : 0f;
                    tex.SetPixel(x, y, new Color(fill.r, fill.g, fill.b, fill.a * a));
                }
            }
            tex.Apply();
            return tex;
        }

        static void OnScene(SceneView view)
        {
            if (!AmbientMenu.IsOn(AmbientMenu.Eyes))
            {
                return;
            }
            var markers = Object.FindObjectsByType<Marker>();
            if (markers == null || markers.Length == 0)
            {
                return;
            }
            Ensure();

            Vector2 look = Event.current.mousePosition;
            Handles.BeginGUI();
            foreach (var m in markers)
            {
                if (m == null)
                {
                    continue;
                }
                Vector2 gp = HandleUtility.WorldToGUIPoint(m.transform.position);
                DrawEye(gp, look);
            }
            Handles.EndGUI();
            view.Repaint();
        }

        static void DrawEye(Vector2 center, Vector2 look)
        {
            float er = 8f;
            float ir = 4f;
            GUI.DrawTexture(new Rect(center.x - er, center.y - er, er * 2f, er * 2f), sclera);
            Vector2 dir = look - center;
            if (dir.sqrMagnitude > 1f)
            {
                dir = dir.normalized;
            }
            Vector2 p = center + dir * (er - ir - 1f);
            GUI.DrawTexture(new Rect(p.x - ir, p.y - ir, ir * 2f, ir * 2f), iris);
        }
    }
}
