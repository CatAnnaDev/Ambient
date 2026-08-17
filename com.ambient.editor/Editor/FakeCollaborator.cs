using UnityEditor;
using UnityEngine;

namespace Ambient
{
    [InitializeOnLoad]
    static class FakeCollaborator
    {
        static Texture2D dot;
        static Vector2 pos;
        static Vector2 target;
        static bool seeded;
        static double nextMove;
        static GameObject framed;
        static double frameUntil;

        static FakeCollaborator()
        {
            SceneView.duringSceneGui += OnScene;
        }

        static void Ensure()
        {
            if (dot == null)
            {
                dot = Disc(14, new Color(0.62f, 0.12f, 0.72f, 1f));
            }
        }

        static void OnScene(SceneView view)
        {
            if (!AmbientMenu.IsOn(AmbientMenu.Intruder))
            {
                return;
            }
            Ensure();

            double t = EditorApplication.timeSinceStartup;
            float w = view.position.width;
            float h = view.position.height;

            if (!seeded)
            {
                seeded = true;
                pos = new Vector2(w * 0.5f, h * 0.5f);
                target = pos;
                nextMove = t + 2.0;
            }

            if (t >= nextMove)
            {
                target = new Vector2(Random.Range(w * 0.1f, w * 0.9f), Random.Range(h * 0.1f, h * 0.9f));
                nextMove = t + Random.Range(1.5f, 4.5f);
                if (Random.value < 0.4f)
                {
                    framed = RandomSceneObject();
                    frameUntil = t + Random.Range(1.5f, 3f);
                }
            }
            pos = Vector2.Lerp(pos, target, 0.03f);

            if (framed != null && t < frameUntil)
            {
                var rend = framed.GetComponentInChildren<Renderer>();
                if (rend != null)
                {
                    var pc = Handles.color;
                    Handles.color = new Color(0.62f, 0.12f, 0.72f, 0.75f);
                    Handles.DrawWireCube(rend.bounds.center, rend.bounds.size);
                    Handles.color = pc;
                }
            }

            Handles.BeginGUI();
            var prev = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.9f);
            GUI.DrawTexture(new Rect(pos.x - 7f, pos.y - 7f, 14f, 14f), dot);
            var style = new GUIStyle(EditorStyles.boldLabel);
            style.normal.textColor = new Color(0.72f, 0.16f, 0.82f, 0.9f);
            GUI.Label(new Rect(pos.x + 10f, pos.y - 6f, 80f, 18f), "???", style);
            GUI.color = prev;
            Handles.EndGUI();

            view.Repaint();
        }

        static GameObject RandomSceneObject()
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                return null;
            }
            var roots = scene.GetRootGameObjects();
            if (roots.Length == 0)
            {
                return null;
            }
            return roots[Random.Range(0, roots.Length)];
        }

        static Texture2D Disc(int size, Color c)
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
                    tex.SetPixel(x, y, new Color(c.r, c.g, c.b, c.a * a));
                }
            }
            tex.Apply();
            return tex;
        }
    }
}
