using UnityEditor;
using UnityEngine;

namespace Ambient
{
    class SaveWatcher : UnityEditor.AssetModificationProcessor
    {
        static string[] OnWillSaveAssets(string[] paths)
        {
            if (AmbientMenu.IsOn(AmbientMenu.Reactive) && Random.value < 0.6f)
            {
                ReactiveHaunt.OnSave();
            }
            return paths;
        }
    }

    [InitializeOnLoad]
    static class ReactiveHaunt
    {
        static double selReactAt;
        static string selName;

        static ReactiveHaunt()
        {
            Selection.selectionChanged += OnSelection;
            EditorApplication.playModeStateChanged += OnPlay;
            EditorApplication.update += Tick;
        }

        static bool On => AmbientMenu.IsOn(AmbientMenu.Reactive);

        public static void OnSave()
        {
            if (!On)
            {
                return;
            }
            Director.Provoke(Cryptic.Any(Cryptic.Save), 0.7f);
        }

        static void OnSelection()
        {
            if (!On)
            {
                return;
            }
            var go = Selection.activeGameObject;
            if (go == null)
            {
                return;
            }
            selName = go.name;
            selReactAt = EditorApplication.timeSinceStartup + Random.Range(0.8f, 1.9f);
        }

        static void OnPlay(PlayModeStateChange state)
        {
            if (!On)
            {
                return;
            }
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                Director.Provoke(Cryptic.Any(Cryptic.Play), 0.95f);
            }
        }

        static void Tick()
        {
            if (!On)
            {
                selReactAt = 0.0;
                return;
            }
            double t = EditorApplication.timeSinceStartup;
            if (selReactAt > 0.0 && t >= selReactAt)
            {
                selReactAt = 0.0;
                if (!string.IsNullOrEmpty(selName) && Random.value < 0.5f)
                {
                    Director.Provoke(Cryptic.Selection(selName), 0.35f);
                }
            }
        }
    }
}
