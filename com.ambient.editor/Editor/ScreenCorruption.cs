using UnityEditor;
using UnityEngine;

namespace Ambient
{
    [InitializeOnLoad]
    static class ScreenCorruption
    {
        static Texture2D scan;
        static Texture2D vignette;

        static ScreenCorruption()
        {
            SceneView.duringSceneGui += OnScene;
        }

        static void Ensure()
        {
            if (scan == null)
            {
                scan = new Texture2D(2, 4, TextureFormat.RGBA32, false);
                scan.hideFlags = HideFlags.HideAndDontSave;
                scan.wrapMode = TextureWrapMode.Repeat;
                scan.filterMode = FilterMode.Point;
                for (int y = 0; y < 4; y++)
                {
                    float a = (y < 2) ? 0.0f : 0.5f;
                    scan.SetPixel(0, y, new Color(0f, 0f, 0f, a));
                    scan.SetPixel(1, y, new Color(0f, 0f, 0f, a));
                }
                scan.Apply();
            }
            if (vignette == null)
            {
                const int s = 64;
                vignette = new Texture2D(s, s, TextureFormat.RGBA32, false);
                vignette.hideFlags = HideFlags.HideAndDontSave;
                float r = s * 0.5f;
                for (int y = 0; y < s; y++)
                {
                    for (int x = 0; x < s; x++)
                    {
                        float dx = (x - r + 0.5f) / r;
                        float dy = (y - r + 0.5f) / r;
                        float d = Mathf.Sqrt(dx * dx + dy * dy);
                        float a = Mathf.Clamp01((d - 0.55f) / 0.45f);
                        vignette.SetPixel(x, y, new Color(0f, 0f, 0f, a * a));
                    }
                }
                vignette.Apply();
            }
        }

        static void OnScene(SceneView view)
        {
            if (!AmbientMenu.IsOn(AmbientMenu.Screen))
            {
                return;
            }
            Ensure();

            float w = view.position.width;
            float h = view.position.height;
            float inten = Mathf.Clamp01(0.10f + Escalation.Intensity * 0.35f + Director.Envelope() * Director.SurgeStrength * 0.9f);
            double t = EditorApplication.timeSinceStartup;

            Handles.BeginGUI();
            var prev = GUI.color;

            GUI.color = new Color(1f, 1f, 1f, 0.10f * inten);
            GUI.DrawTextureWithTexCoords(new Rect(0f, 0f, w, h), scan, new Rect(0f, 0f, w / 2f, h / 4f));

            GUI.color = new Color(1f, 1f, 1f, 0.55f * inten);
            GUI.DrawTexture(new Rect(0f, 0f, w, h), vignette, ScaleMode.StretchToFill, true);

            float tearPhase = Mathf.Repeat((float)t * 0.2f, 7f);
            if (tearPhase < 0.2f)
            {
                float y = Mathf.Repeat((float)t * 850f, h);
                GUI.color = new Color(1f, 1f, 1f, 0.06f);
                GUI.DrawTexture(new Rect(0f, y, w, 3f), Texture2D.whiteTexture);
            }

            GUI.color = prev;
            Handles.EndGUI();
            view.Repaint();
        }
    }
}
