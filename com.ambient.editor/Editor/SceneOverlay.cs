using UnityEditor;
using UnityEngine;

namespace Ambient
{
    [InitializeOnLoad]
    static class SceneOverlay
    {
        static Texture2D sclera;
        static Texture2D iris;

        static SceneOverlay()
        {
            SceneView.duringSceneGui += OnScene;
        }

        static void Ensure()
        {
            if (sclera != null && iris != null)
            {
                return;
            }
            sclera = Disc(64, new Color(0.93f, 0.9f, 0.86f, 1f), new Color(0.5f, 0.06f, 0.06f, 1f));
            iris = Disc(48, new Color(0.04f, 0.02f, 0.02f, 1f), new Color(0.7f, 0.04f, 0.04f, 1f));
        }

        static Texture2D Disc(int size, Color fill, Color rim)
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
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    Color c;
                    if (dist > r)
                    {
                        c = new Color(fill.r, fill.g, fill.b, 0f);
                    }
                    else if (dist > r - 2.4f)
                    {
                        c = rim;
                    }
                    else
                    {
                        c = fill;
                    }
                    tex.SetPixel(x, y, c);
                }
            }
            tex.Apply();
            return tex;
        }

        static void OnScene(SceneView view)
        {
            if (!AmbientMenu.IsOn(AmbientMenu.Overlay))
            {
                return;
            }
            Ensure();

            var e = Event.current;
            Vector2 look = e.mousePosition;
            float width = view.position.width;
            double t = EditorApplication.timeSinceStartup;
            float cy = 58f + Mathf.Sin((float)t * 1.2f) * 3f;
            float cx = width * 0.5f;
            float gap = 44f;
            bool blink = Mathf.Repeat((float)t, 4.4f) < 0.13f;

            Handles.BeginGUI();
            DrawEye(new Vector2(cx - gap, cy), look, blink);
            DrawEye(new Vector2(cx + gap, cy), look, blink);
            Handles.EndGUI();

            view.Repaint();
        }

        static void DrawEye(Vector2 center, Vector2 look, bool blink)
        {
            float er = 24f;
            float ir = 10f;

            if (blink)
            {
                EditorGUI.DrawRect(new Rect(center.x - er, center.y - 2f, er * 2f, 4f), new Color(0.45f, 0.06f, 0.06f, 1f));
                return;
            }

            GUI.DrawTexture(new Rect(center.x - er, center.y - er, er * 2f, er * 2f), sclera);

            Vector2 dir = look - center;
            if (dir.sqrMagnitude > 1f)
            {
                dir = dir.normalized;
            }
            Vector2 p = center + dir * (er - ir - 3f);
            GUI.DrawTexture(new Rect(p.x - ir, p.y - ir, ir * 2f, ir * 2f), iris);
        }
    }
}
