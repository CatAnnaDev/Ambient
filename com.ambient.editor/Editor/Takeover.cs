using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ambient
{
    [InitializeOnLoad]
    static class Takeover
    {
        static readonly List<VisualElement> blacks = new List<VisualElement>();
        static IMGUIContainer stage;
        static bool active;
        static string message;

        static Takeover()
        {
            Director.OnSurge += OnSurge;
            EditorApplication.update += Tick;
        }

        static void OnSurge()
        {
            if (active || !Director.IsTakeover)
            {
                return;
            }
            if (!AmbientMenu.IsOn(AmbientMenu.Presence) && !AmbientMenu.IsOn(AmbientMenu.Screen))
            {
                return;
            }
            Begin();
        }

        static void Begin()
        {
            message = Director.SurgeMessage ?? "";
            var wins = Resources.FindObjectsOfTypeAll<EditorWindow>();
            EditorWindow biggest = null;
            float bestArea = 0f;
            foreach (var w in wins)
            {
                if (w == null || w is AmbientPreview)
                {
                    continue;
                }
                var root = w.rootVisualElement;
                if (root == null || root.panel == null)
                {
                    continue;
                }
                var b = new VisualElement();
                b.style.position = Position.Absolute;
                b.style.left = 0f;
                b.style.top = 0f;
                b.style.right = 0f;
                b.style.bottom = 0f;
                b.style.backgroundColor = new Color(0f, 0f, 0f, 1f);
                b.style.opacity = 0f;
                b.pickingMode = PickingMode.Ignore;
                root.Add(b);
                blacks.Add(b);

                float area = w.position.width * w.position.height;
                if (area > bestArea)
                {
                    bestArea = area;
                    biggest = w;
                }
            }

            if (biggest != null)
            {
                stage = new IMGUIContainer(DrawStage);
                stage.style.position = Position.Absolute;
                stage.style.left = 0f;
                stage.style.top = 0f;
                stage.style.right = 0f;
                stage.style.bottom = 0f;
                stage.pickingMode = PickingMode.Ignore;
                biggest.rootVisualElement.Add(stage);
            }
            active = true;
        }

        static void DrawStage()
        {
            float env = Director.Envelope();
            var rect = stage.contentRect;
            if (rect.width < 1f || rect.height < 1f)
            {
                return;
            }
            var fig = Figures.Silhouette();
            float gh = rect.height * 0.9f;
            float gw = gh * 0.5f;

            var prev = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, env);
            GUI.DrawTexture(new Rect(rect.width * 0.5f - gw * 0.5f, rect.height * 0.5f - gh * 0.5f, gw, gh), fig, ScaleMode.ScaleToFit, true);

            int total = message.Length;
            int n = Mathf.Clamp(Mathf.RoundToInt(env * total), 0, total);
            string shown = message.Substring(0, n);
            var style = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.RoundToInt(Mathf.Clamp(rect.height * 0.045f, 14f, 42f)),
            };
            style.normal.textColor = new Color(0.85f, 0.06f, 0.06f, env);
            GUI.Label(new Rect(0f, rect.height * 0.72f, rect.width, 60f), shown, style);
            GUI.color = prev;
        }

        static void Tick()
        {
            if (!active)
            {
                return;
            }
            float env = Director.Envelope();
            foreach (var b in blacks)
            {
                if (b != null)
                {
                    b.style.opacity = env * 0.92f;
                }
            }
            if (stage != null)
            {
                stage.MarkDirtyRepaint();
            }
            if (!Director.Surging)
            {
                Remove();
            }
        }

        static void Remove()
        {
            foreach (var b in blacks)
            {
                try
                {
                    if (b != null)
                    {
                        b.RemoveFromHierarchy();
                    }
                }
                catch
                {
                }
            }
            blacks.Clear();
            try
            {
                if (stage != null)
                {
                    stage.RemoveFromHierarchy();
                }
            }
            catch
            {
            }
            stage = null;
            active = false;
        }
    }
}
