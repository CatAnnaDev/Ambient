using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ambient
{
    public class GhostWindow : EditorWindow
    {
        public double born;
        public float life;
        public string message;
        public int mode;

        public float Progress => life > 0f ? Mathf.Clamp01((float)((EditorApplication.timeSinceStartup - born) / life)) : 1f;

        void OnGUI()
        {
            float p = Progress;
            float env = p < 0.2f ? p / 0.2f : 1f - (p - 0.2f) / 0.8f;
            var full = new Rect(0f, 0f, position.width, position.height);

            if (mode == 1)
            {
                EditorGUI.DrawRect(full, new Color(0.03f, 0.03f, 0.03f, Mathf.Clamp01(env * 1.5f)));
                var st = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 16,
                    wordWrap = true,
                };
                st.normal.textColor = new Color(0.85f, 0.08f, 0.08f, env);
                GUI.Label(new Rect(12f, 18f, position.width - 24f, position.height - 66f), message, st);

                var br = new Rect(position.width * 0.5f - 32f, position.height - 38f, 64f, 24f);
                var oc = GUI.color;
                GUI.color = new Color(1f, 1f, 1f, env);
                if (GUI.Button(br, "OK"))
                {
                    Close();
                }
                GUI.color = oc;
            }
            else if (mode == 2)
            {
                EditorGUI.DrawRect(full, new Color(0f, 0f, 0f, Mathf.Clamp01(p * 1.2f) * 0.92f));
                var rig = Figures.Silhouette();
                float rscale = Mathf.Lerp(0.2f, 1.4f, p * p);
                float rfh = position.height * rscale;
                float rfw = rfh * 0.5f;
                var roc = GUI.color;
                GUI.color = new Color(1f, 1f, 1f, Mathf.Clamp01(p * 1.6f));
                GUI.DrawTexture(new Rect(position.width * 0.5f - rfw * 0.5f, position.height * 0.62f - rfh * 0.5f, rfw, rfh), rig, ScaleMode.ScaleToFit, true);
                GUI.color = roc;
            }
            else
            {
                EditorGUI.DrawRect(full, new Color(0f, 0f, 0f, 0.82f * env));
                var fig = Figures.Silhouette();
                float fh = position.height * 0.92f;
                float fw = fh * 0.5f;
                var oc = GUI.color;
                GUI.color = new Color(1f, 1f, 1f, env);
                GUI.DrawTexture(new Rect(position.width * 0.5f - fw * 0.5f, position.height * 0.5f - fh * 0.5f, fw, fh), fig, ScaleMode.ScaleToFit, true);
                GUI.color = oc;
            }
        }
    }

    [InitializeOnLoad]
    static class GhostSpawner
    {
        static readonly List<GhostWindow> open = new List<GhostWindow>();

        static GhostSpawner()
        {
            Director.OnSurge += OnSurge;
            EditorApplication.update += Tick;
        }

        static void Tick()
        {
            for (int i = open.Count - 1; i >= 0; i--)
            {
                var g = open[i];
                if (g == null)
                {
                    open.RemoveAt(i);
                    continue;
                }
                if (g.Progress >= 1f)
                {
                    try
                    {
                        g.Close();
                    }
                    catch
                    {
                    }
                    open.RemoveAt(i);
                }
                else
                {
                    g.Repaint();
                }
            }
        }

        static void OnSurge()
        {
            if (!AmbientMenu.IsOn(AmbientMenu.Presence))
            {
                return;
            }
            int cap = Director.IsTakeover ? 10 : 4;
            if (open.Count >= cap)
            {
                return;
            }
            Rect main = EditorBounds();

            if (Director.IsTakeover)
            {
                Make(new Vector2(main.x, main.y), new Vector2(main.width, main.height), null, 2, 1.3f);
                var dsize = new Vector2(360f, 150f);
                var dpos = new Vector2(main.center.x - dsize.x * 0.5f, main.center.y - dsize.y * 0.5f);
                Make(dpos, dsize, Cryptic.Pick(), 1, 2.6f);
                for (int i = 0; i < 5 && open.Count < cap; i++)
                {
                    var s = new Vector2(Random.Range(130f, 220f), Random.Range(240f, 420f));
                    Make(RandPos(main, s), s, null, 0, Random.Range(1.6f, 2.8f));
                }
            }
            else if (Random.value < Director.SurgeStrength * 0.5f)
            {
                var s = new Vector2(Random.Range(120f, 200f), Random.Range(260f, 360f));
                Make(RandPos(main, s), s, null, 0, Random.Range(1.2f, 2.2f));
            }
        }

        static void Make(Vector2 pos, Vector2 size, string msg, int mode, float life)
        {
            var w = ScriptableObject.CreateInstance<GhostWindow>();
            w.born = EditorApplication.timeSinceStartup;
            w.life = life;
            w.message = msg;
            w.mode = mode;
            w.ShowPopup();
            w.position = new Rect(pos, size);
            open.Add(w);
        }

        static Vector2 RandPos(Rect main, Vector2 size)
        {
            float x = main.x + Random.Range(-size.x * 0.2f, main.width - size.x * 0.8f);
            float y = main.y + Random.Range(-size.y * 0.1f, main.height - size.y * 0.6f);
            return new Vector2(x, y);
        }

        static Rect EditorBounds()
        {
            var r = new Rect(200f, 200f, 1000f, 700f);
            bool has = false;
            foreach (var w in Resources.FindObjectsOfTypeAll<EditorWindow>())
            {
                if (w == null)
                {
                    continue;
                }
                var p = w.position;
                if (p.width < 60f || p.height < 60f)
                {
                    continue;
                }
                if (!has)
                {
                    r = p;
                    has = true;
                }
                else
                {
                    float xMin = Mathf.Min(r.xMin, p.xMin);
                    float yMin = Mathf.Min(r.yMin, p.yMin);
                    float xMax = Mathf.Max(r.xMax, p.xMax);
                    float yMax = Mathf.Max(r.yMax, p.yMax);
                    r = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
                }
            }
            return r;
        }
    }
}
