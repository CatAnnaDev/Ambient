using UnityEditor;
using UnityEngine;

namespace Ambient
{
    [InitializeOnLoad]
    static class ConsoleFlood
    {
        static int remaining;
        static double nextLine;

        static ConsoleFlood()
        {
            Director.OnSurge += OnSurge;
            EditorApplication.update += Tick;
        }

        static void OnSurge()
        {
            if (!Director.IsTakeover || !AmbientMenu.IsOn(AmbientMenu.Whispers))
            {
                return;
            }
            remaining = Random.Range(16, 28);
            nextLine = 0.0;
        }

        static void Tick()
        {
            if (remaining <= 0)
            {
                return;
            }
            double t = EditorApplication.timeSinceStartup;
            if (nextLine <= 0.0)
            {
                nextLine = t + 0.04;
                return;
            }
            if (t >= nextLine)
            {
                nextLine = t + 0.04;
                float r = Random.value;
                var type = r < 0.45f ? LogType.Warning : (r < 0.75f ? LogType.Error : LogType.Log);
                AmbientLog.Say(Cryptic.Pick(), type);
                remaining--;
            }
        }
    }
}
