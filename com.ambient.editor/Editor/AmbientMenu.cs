using UnityEditor;

namespace Ambient
{
    public static class AmbientMenu
    {
        public const string Overlay = "ambient.overlay";
        public const string Names = "ambient.names";
        public const string HauntEditor = "ambient.haunt_editor";
        public const string Presence = "ambient.presence";
        public const string Whispers = "ambient.whispers";
        public const string Reactive = "ambient.reactive";
        public const string Eyes = "ambient.eyes";
        public const string Console = "ambient.console";
        public const string Screen = "ambient.screen";
        public const string Intruder = "ambient.intruder";
        public const string Spawns = "ambient.spawns";
        public const string Physics = "ambient.physics";

        static readonly string[] All =
        {
            Names,
            HauntEditor,
            Presence,
            Whispers,
            Reactive,
            Console,
            Intruder,
            Spawns,
            Physics,
        };

        public static bool IsOn(string key) => EditorPrefs.GetBool(key, false);

        public static void Set(string key, bool on) => EditorPrefs.SetBool(key, on);

        static void RepaintAll()
        {
            SceneView.RepaintAll();
            EditorApplication.RepaintHierarchyWindow();
            EditorApplication.RepaintProjectWindow();
        }

        static void Toggle(string key)
        {
            EditorPrefs.SetBool(key, !IsOn(key));
            RepaintAll();
        }

        public static void SetAll(bool on)
        {
            foreach (var k in All)
            {
                EditorPrefs.SetBool(k, on);
            }
            if (!on)
            {
                AmbientPreview.CloseIfOpen();
            }
            RepaintAll();
        }

        [MenuItem("Ambient/Enable All", false, 0)]
        static void EnableAll() => SetAll(true);

        [MenuItem("Ambient/Disable All", false, 1)]
        static void DisableAll() => SetAll(false);

        [MenuItem("Ambient/Manifest", false, 12)]
        static void Manifest() => Director.Provoke(Cryptic.Pick(), 0.7f);

        [MenuItem("Ambient/Takeover", false, 13)]
        static void Takeover() => Director.Provoke(Cryptic.Pick(), 1f, true);

        [MenuItem("Ambient/Haunted Names", false, 20)]
        static void TNames() => Toggle(Names);
        [MenuItem("Ambient/Haunted Names", true)]
        static bool VNames() { Menu.SetChecked("Ambient/Haunted Names", IsOn(Names)); return true; }

        [MenuItem("Ambient/Haunt the Editor", false, 21)]
        static void THaunt() => Toggle(HauntEditor);
        [MenuItem("Ambient/Haunt the Editor", true)]
        static bool VHaunt() { Menu.SetChecked("Ambient/Haunt the Editor", IsOn(HauntEditor)); return true; }

        [MenuItem("Ambient/Presence", false, 22)]
        static void TPresence() => Toggle(Presence);
        [MenuItem("Ambient/Presence", true)]
        static bool VPresence() { Menu.SetChecked("Ambient/Presence", IsOn(Presence)); return true; }

        [MenuItem("Ambient/Physics", false, 23)]
        static void TPhysics() => Toggle(Physics);
        [MenuItem("Ambient/Physics", true)]
        static bool VPhysics() { Menu.SetChecked("Ambient/Physics", IsOn(Physics)); return true; }

        [MenuItem("Ambient/Spawns", false, 24)]
        static void TSpawns() => Toggle(Spawns);
        [MenuItem("Ambient/Spawns", true)]
        static bool VSpawns() { Menu.SetChecked("Ambient/Spawns", IsOn(Spawns)); return true; }

        [MenuItem("Ambient/Intruder", false, 25)]
        static void TIntruder() => Toggle(Intruder);
        [MenuItem("Ambient/Intruder", true)]
        static bool VIntruder() { Menu.SetChecked("Ambient/Intruder", IsOn(Intruder)); return true; }

        [MenuItem("Ambient/Whispers", false, 26)]
        static void TWhispers() => Toggle(Whispers);
        [MenuItem("Ambient/Whispers", true)]
        static bool VWhispers() { Menu.SetChecked("Ambient/Whispers", IsOn(Whispers)); return true; }

        [MenuItem("Ambient/Reactive", false, 27)]
        static void TReactive() => Toggle(Reactive);
        [MenuItem("Ambient/Reactive", true)]
        static bool VReactive() { Menu.SetChecked("Ambient/Reactive", IsOn(Reactive)); return true; }

        [MenuItem("Ambient/Console", false, 28)]
        static void TConsole() => Toggle(Console);
        [MenuItem("Ambient/Console", true)]
        static bool VConsole() { Menu.SetChecked("Ambient/Console", IsOn(Console)); return true; }

        [MenuItem("Ambient/Screen Corruption", false, 40)]
        static void TScreen() => Toggle(Screen);
        [MenuItem("Ambient/Screen Corruption", true)]
        static bool VScreen() { Menu.SetChecked("Ambient/Screen Corruption", IsOn(Screen)); return true; }

        [MenuItem("Ambient/Scene Overlay", false, 41)]
        static void TOverlay() => Toggle(Overlay);
        [MenuItem("Ambient/Scene Overlay", true)]
        static bool VOverlay() { Menu.SetChecked("Ambient/Scene Overlay", IsOn(Overlay)); return true; }

        [MenuItem("Ambient/Watching Objects", false, 42)]
        static void TEyes() => Toggle(Eyes);
        [MenuItem("Ambient/Watching Objects", true)]
        static bool VEyes() { Menu.SetChecked("Ambient/Watching Objects", IsOn(Eyes)); return true; }

        [MenuItem("Ambient/Camera Preview", false, 60)]
        static void OpenPreview() => AmbientPreview.Open();

        [MenuItem("Ambient/Tag Selected", false, 61)]
        static void TagSelected()
        {
            foreach (var go in Selection.gameObjects)
            {
                if (go.GetComponent<Marker>() == null)
                {
                    Undo.AddComponent<Marker>(go);
                }
            }
        }

        [MenuItem("Ambient/Tag Selected", true)]
        static bool VTag() => Selection.gameObjects.Length > 0;

        [MenuItem("Ambient/Reset Panels", false, 62)]
        static void ResetPanels() => PanelPhysics.Reset();
    }
}
