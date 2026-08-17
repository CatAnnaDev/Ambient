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
        };

        public static bool IsOn(string key) => EditorPrefs.GetBool(key, false);

        public static void Set(string key, bool on) => EditorPrefs.SetBool(key, on);

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
            SceneView.RepaintAll();
            EditorApplication.RepaintHierarchyWindow();
            EditorApplication.RepaintProjectWindow();
        }
    }
}
