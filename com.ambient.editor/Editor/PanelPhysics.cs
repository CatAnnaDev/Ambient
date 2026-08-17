using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ambient
{
    [InitializeOnLoad]
    static class PanelPhysics
    {
        class Faller
        {
            public FallingPanel win;
            public EditorWindow origin;
            public float vx;
            public float vy;
            public bool rested;
            public double restedAt;
        }

        static readonly List<Faller> fallers = new List<Faller>();
        static readonly HashSet<EditorWindow> down = new HashSet<EditorWindow>();
        static readonly HashSet<EditorWindow> registered = new HashSet<EditorWindow>();
        static double nextScan;

        static PanelPhysics()
        {
            EditorApplication.update += Tick;
            Director.OnSurge += OnSurge;
        }

        public static bool IsFalling(EditorWindow w) => w != null && down.Contains(w);

        public static void Reset() => RestoreAll();

        static void Tick()
        {
            if (!AmbientMenu.IsOn(AmbientMenu.Physics))
            {
                if (fallers.Count > 0)
                {
                    RestoreAll();
                }
                return;
            }

            double t = EditorApplication.timeSinceStartup;
            if (t >= nextScan)
            {
                nextScan = t + 1.0;
                Scan();
            }
            if (fallers.Count == 0)
            {
                return;
            }

            var b = EditorBounds();
            const float dt = 0.016f;
            const float g = 2600f;

            for (int i = fallers.Count - 1; i >= 0; i--)
            {
                var f = fallers[i];
                if (f.win == null)
                {
                    Restore(f);
                    fallers.RemoveAt(i);
                    continue;
                }
                var p = f.win.position;
                f.vy += g * dt;
                p.x += f.vx * dt;
                p.y += f.vy * dt;

                float floor = b.yMax - p.height;
                if (p.y >= floor)
                {
                    p.y = floor;
                    if (f.vy > 80f)
                    {
                        f.vy = -f.vy * 0.42f;
                        f.vx *= 0.7f;
                    }
                    else
                    {
                        f.vy = 0f;
                        f.vx *= 0.6f;
                        if (!f.rested)
                        {
                            f.rested = true;
                            f.restedAt = t;
                        }
                    }
                }
                if (p.x < b.xMin)
                {
                    p.x = b.xMin;
                    f.vx = -f.vx * 0.5f;
                }
                if (p.x + p.width > b.xMax)
                {
                    p.x = b.xMax - p.width;
                    f.vx = -f.vx * 0.5f;
                }

                f.win.position = p;
                f.win.Repaint();

                if (f.rested && t - f.restedAt > 9.0)
                {
                    Restore(f);
                    fallers.RemoveAt(i);
                }
            }
        }

        static void OnSurge()
        {
            if (!AmbientMenu.IsOn(AmbientMenu.Physics) || !Director.IsTakeover)
            {
                return;
            }
            var wins = RealPanels();
            for (int i = 0; i < 3 && wins.Count > 0; i++)
            {
                var w = wins[Random.Range(0, wins.Count)];
                wins.Remove(w);
                Detach(w);
            }
        }

        static void Scan()
        {
            foreach (var w in RealPanels())
            {
                if (registered.Contains(w))
                {
                    continue;
                }
                registered.Add(w);
                var cap = w;
                w.rootVisualElement.RegisterCallback<PointerDownEvent>(e => OnClick(cap), TrickleDown.TrickleDown);
            }
        }

        static void OnClick(EditorWindow w)
        {
            if (!AmbientMenu.IsOn(AmbientMenu.Physics) || down.Contains(w))
            {
                return;
            }
            float chance = 0.12f + Director.Envelope() * Director.SurgeStrength * 0.7f;
            if (Random.value > chance)
            {
                return;
            }
            Detach(w);
        }

        static void Detach(EditorWindow w)
        {
            if (w == null || down.Contains(w))
            {
                return;
            }
            var r = w.rootVisualElement;
            if (r == null || r.panel == null)
            {
                return;
            }
            var rect = w.position;
            var fp = ScriptableObject.CreateInstance<FallingPanel>();
            fp.label = w.titleContent != null ? w.titleContent.text : "";
            fp.ShowPopup();
            fp.position = rect;
            r.style.display = DisplayStyle.None;
            down.Add(w);
            fallers.Add(new Faller
            {
                win = fp,
                origin = w,
                vx = Random.Range(-60f, 60f),
                vy = Random.Range(-40f, 90f),
            });
        }

        static void Restore(Faller f)
        {
            if (f.win != null)
            {
                try
                {
                    f.win.Close();
                }
                catch
                {
                }
            }
            if (f.origin != null)
            {
                down.Remove(f.origin);
                var r = f.origin.rootVisualElement;
                if (r != null)
                {
                    try
                    {
                        r.style.display = DisplayStyle.Flex;
                    }
                    catch
                    {
                    }
                }
            }
        }

        static void RestoreAll()
        {
            for (int i = fallers.Count - 1; i >= 0; i--)
            {
                Restore(fallers[i]);
            }
            fallers.Clear();
            down.Clear();
        }

        static List<EditorWindow> RealPanels()
        {
            var list = new List<EditorWindow>();
            foreach (var w in Resources.FindObjectsOfTypeAll<EditorWindow>())
            {
                if (w == null || w is AmbientPreview || w is FallingPanel || w is GhostWindow)
                {
                    continue;
                }
                var r = w.rootVisualElement;
                if (r == null || r.panel == null)
                {
                    continue;
                }
                var p = w.position;
                if (p.width < 120f || p.height < 80f)
                {
                    continue;
                }
                list.Add(w);
            }
            return list;
        }

        static Rect EditorBounds()
        {
            var r = new Rect(0f, 0f, 1400f, 900f);
            bool has = false;
            foreach (var w in RealPanels())
            {
                var p = w.position;
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
