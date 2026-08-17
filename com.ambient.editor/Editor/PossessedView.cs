using UnityEditor;
using UnityEngine;

namespace Ambient
{
    [InitializeOnLoad]
    static class PossessedView
    {
        static Texture2D noise;
        static double nextFlicker;
        static bool flickering;
        static double flickerStart;

        static PossessedView()
        {
            SceneView.duringSceneGui += OnScene;
            Director.OnSurge += OnManifest;
        }

        static void OnManifest()
        {
            flickering = true;
            flickerStart = EditorApplication.timeSinceStartup;
        }

        static void Ensure()
        {
            if (noise == null)
            {
                noise = BuildNoise(120, 120);
            }
        }

        static void OnScene(SceneView view)
        {
            if (!AmbientMenu.IsOn(AmbientMenu.Presence))
            {
                return;
            }
            Ensure();

            double t = EditorApplication.timeSinceStartup;
            if (nextFlicker <= 0.0)
            {
                nextFlicker = t + Random.Range(18f, 40f);
            }
            else if (!flickering && t >= nextFlicker)
            {
                flickering = true;
                flickerStart = t;
            }

            if (!flickering)
            {
                return;
            }

            float fp = (float)((t - flickerStart) / 0.18);
            if (fp >= 1f)
            {
                flickering = false;
                nextFlicker = t + Random.Range(22f, 55f);
            }
            else
            {
                Handles.BeginGUI();
                var prev = GUI.color;
                GUI.color = new Color(1f, 1f, 1f, 0.10f + 0.12f * Mathf.Abs(Mathf.Sin(fp * 34f)));
                GUI.DrawTexture(new Rect(0f, 0f, view.position.width, view.position.height), noise, ScaleMode.StretchToFill, true);
                GUI.color = prev;
                Handles.EndGUI();
            }

            view.Repaint();
        }

        static Texture2D BuildNoise(int w, int h)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.hideFlags = HideFlags.HideAndDontSave;
            var rng = new System.Random(1337);
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float n = (float)rng.NextDouble();
                    float scan = (y % 3 == 0) ? 0.5f : 0f;
                    float a = Mathf.Clamp01(n * 0.55f + scan);
                    tex.SetPixel(x, y, new Color(0f, 0f, 0f, a));
                }
            }
            tex.Apply();
            return tex;
        }
    }
}
