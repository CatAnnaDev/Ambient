using UnityEditor;
using UnityEngine;

namespace Ambient
{
    [InitializeOnLoad]
    static class Whispers
    {
        static Whispers()
        {
            Director.OnSurge += OnManifest;
        }

        static void OnManifest()
        {
            if (!AmbientMenu.IsOn(AmbientMenu.Whispers))
            {
                return;
            }
            var type = Random.value < 0.5f ? LogType.Warning : LogType.Log;
            AmbientLog.Say(Director.SurgeMessage, type);
        }
    }
}
