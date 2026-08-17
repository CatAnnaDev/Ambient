using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ambient
{
    [InitializeOnLoad]
    static class EditorHaunt
    {
        enum Kind { Shudder, Tilt, Drop, Vanish, Invert, Blackout }

        static EditorWindow target;
        static VisualElement root;
        static VisualElement blackout;
        static Kind kind;
        static double start;
        static float duration;
        static bool active;

        public static EditorWindow Current => active ? target : null;

        static EditorHaunt()
        {
            EditorApplication.update += Tick;
            Director.OnSurge += Poke;
        }

        static void Tick()
        {
            if (!AmbientMenu.IsOn(AmbientMenu.HauntEditor))
            {
                if (active)
                {
                    Reset();
                }
                return;
            }
            if (!active)
            {
                return;
            }
            double t = EditorApplication.timeSinceStartup;
            float p = duration > 0f ? (float)((t - start) / duration) : 1f;
            if (p >= 1f || target == null || root == null || root.panel == null)
            {
                Reset();
                return;
            }
            Apply(p);
        }

        static Kind PickKind()
        {
            float hi = 0.05f + Escalation.Intensity * 0.12f;
            float r = Random.value;
            if (r < hi) return Kind.Invert;
            if (r < hi * 2f) return Kind.Blackout;
            return (Kind)Random.Range(0, 4);
        }

        public static void Poke()
        {
            if (active)
            {
                return;
            }
            if (!AmbientMenu.IsOn(AmbientMenu.HauntEditor))
            {
                return;
            }
            Begin(EditorApplication.timeSinceStartup);
        }

        static void Begin(double t)
        {
            target = PickWindow();
            if (target == null)
            {
                return;
            }
            root = target.rootVisualElement;
            if (root == null)
            {
                target = null;
                return;
            }
            kind = PickKind();
            start = t;
            switch (kind)
            {
                case Kind.Vanish: duration = 0.5f; break;
                case Kind.Blackout: duration = 0.45f; break;
                case Kind.Invert: duration = 0.9f; break;
                default: duration = 0.7f; break;
            }
            active = true;

            if (kind == Kind.Blackout)
            {
                blackout = new VisualElement();
                blackout.style.position = Position.Absolute;
                blackout.style.left = 0f;
                blackout.style.top = 0f;
                blackout.style.right = 0f;
                blackout.style.bottom = 0f;
                blackout.style.backgroundColor = new Color(0f, 0f, 0f, 1f);
                blackout.pickingMode = PickingMode.Ignore;
                root.Add(blackout);
            }
        }

        static void Apply(float p)
        {
            switch (kind)
            {
                case Kind.Shudder:
                    float dx = Mathf.Sin(p * Mathf.PI * 12f) * 5f * (1f - p);
                    float dy = Mathf.Cos(p * Mathf.PI * 9f) * 3f * (1f - p);
                    root.style.translate = new StyleTranslate(new Translate(new Length(dx), new Length(dy)));
                    break;
                case Kind.Tilt:
                    root.style.rotate = new StyleRotate(new Rotate(new Angle(Mathf.Sin(p * Mathf.PI) * 3.5f)));
                    break;
                case Kind.Drop:
                    root.style.translate = new StyleTranslate(new Translate(new Length(0f), new Length(Mathf.Sin(p * Mathf.PI) * 16f)));
                    break;
                case Kind.Vanish:
                    root.style.display = (p > 0.2f && p < 0.55f) ? DisplayStyle.None : DisplayStyle.Flex;
                    break;
                case Kind.Invert:
                    root.style.rotate = new StyleRotate(new Rotate(new Angle(Mathf.Sin(p * Mathf.PI) * 180f)));
                    break;
                case Kind.Blackout:
                    break;
            }
        }

        static void Reset()
        {
            try
            {
                if (root != null)
                {
                    root.style.translate = new StyleTranslate(new Translate(new Length(0f), new Length(0f)));
                    root.style.rotate = new StyleRotate(new Rotate(new Angle(0f)));
                    root.style.display = DisplayStyle.Flex;
                }
                if (blackout != null)
                {
                    blackout.RemoveFromHierarchy();
                    blackout = null;
                }
            }
            catch
            {
            }
            active = false;
            target = null;
            root = null;
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
                if (pos.width < 80f || pos.height < 60f)
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
