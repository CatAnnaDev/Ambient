using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ambient
{
    [InitializeOnLoad]
    static class Apparition
    {
        static IMGUIContainer current;
        static float opacity;
        static float fw;
        static float fh;
        static double start;
        static float duration;
        static float strength;
        static bool active;

        static Apparition()
        {
            Director.OnSurge += OnSurge;
            EditorApplication.update += Tick;
        }

        static void OnSurge()
        {
            if (!AmbientMenu.IsOn(AmbientMenu.Presence) || active)
            {
                return;
            }
            Spawn(Director.SurgeStrength);
        }

        static void Spawn(float s)
        {
            var host = PickWindow();
            if (host == null)
            {
                return;
            }
            var root = host.rootVisualElement;
            if (root == null || root.panel == null)
            {
                return;
            }

            float pw = host.position.width;
            float ph = host.position.height;
            int approach = SessionState.GetInt("ambient.approach", 0);
            float k = Mathf.Clamp01(approach / 12f);

            fh = Mathf.Clamp(ph * Mathf.Lerp(0.5f, 1.05f, k) * Random.Range(0.85f, 1.1f), 140f, 560f);
            fw = fh * 0.5f;
            float cxp = pw * 0.5f - fw * 0.5f;
            float cyp = ph * 0.5f - fh * 0.5f;
            float spread = Mathf.Lerp(1f, 0.3f, k);
            float x = Mathf.Lerp(cxp, Random.Range(-fw * 0.35f, pw - fw * 0.65f), spread);
            float y = Mathf.Lerp(cyp, Random.Range(-fh * 0.15f, ph - fh * 0.45f), spread);
            SessionState.SetInt("ambient.approach", approach + 1);

            opacity = 0f;
            var ve = new IMGUIContainer(DrawFigure);
            ve.pickingMode = PickingMode.Ignore;
            ve.style.position = Position.Absolute;
            ve.style.left = x;
            ve.style.top = y;
            ve.style.width = fw;
            ve.style.height = fh;
            root.Add(ve);

            current = ve;
            start = EditorApplication.timeSinceStartup;
            duration = Random.Range(2.0f, 3.6f);
            strength = Mathf.Clamp01(0.3f + s * 0.7f);
            active = true;
        }

        static void DrawFigure()
        {
            var fig = Figures.Silhouette();
            if (fig == null)
            {
                return;
            }
            var prev = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, opacity);
            GUI.DrawTexture(new Rect(0f, 0f, fw, fh), fig, ScaleMode.ScaleToFit, true);
            GUI.color = prev;
        }

        static void Tick()
        {
            if (!active)
            {
                return;
            }
            if (!AmbientMenu.IsOn(AmbientMenu.Presence) || current == null || current.panel == null)
            {
                Remove();
                return;
            }
            double t = EditorApplication.timeSinceStartup;
            float p = duration > 0f ? Mathf.Clamp01((float)((t - start) / duration)) : 1f;
            float env = p < 0.25f ? p / 0.25f : 1f - (p - 0.25f) / 0.75f;
            opacity = env * strength * 0.9f;
            current.MarkDirtyRepaint();
            if (p >= 1f)
            {
                Remove();
            }
        }

        static void Remove()
        {
            try
            {
                if (current != null)
                {
                    current.RemoveFromHierarchy();
                }
            }
            catch
            {
            }
            current = null;
            active = false;
        }

        static EditorWindow PickWindow()
        {
            var all = Resources.FindObjectsOfTypeAll<EditorWindow>();
            if (all == null || all.Length == 0)
            {
                return null;
            }
            var pool = new List<EditorWindow>(all.Length);
            foreach (var w in all)
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
                var pos = w.position;
                if (pos.width < 120f || pos.height < 90f)
                {
                    continue;
                }
                pool.Add(w);
            }
            if (pool.Count == 0)
            {
                return null;
            }
            return pool[Random.Range(0, pool.Count)];
        }
    }
}
