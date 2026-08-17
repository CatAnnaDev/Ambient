using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Ambient
{
    static class Control
    {
        [Shortcut("Ambient/Arm", KeyCode.G, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
        static void Arm() => AmbientMenu.SetAll(true);

        [Shortcut("Ambient/Disarm", KeyCode.H, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
        static void Disarm() => AmbientMenu.SetAll(false);

        [Shortcut("Ambient/Manifest", KeyCode.J, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
        static void Manifest() => Director.Provoke(Cryptic.Pick(), 0.7f);

        [Shortcut("Ambient/Takeover", KeyCode.K, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
        static void Takeover() => Director.Provoke(Cryptic.Pick(), 1f, true);

        [Shortcut("Ambient/Watcher", KeyCode.L, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
        static void Watcher() => AmbientPreview.Open();
    }
}
