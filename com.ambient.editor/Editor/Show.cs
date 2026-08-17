using UnityEditor;
using UnityEngine;

namespace Ambient
{
    [InitializeOnLoad]
    static class Show
    {
        static bool running;
        static double startT;
        static double nextProvoke;
        static bool apexFired;

        static Show()
        {
            EditorApplication.update += Tick;
        }

        [MenuItem("Ambient/Run Show", false, 3)]
        static void Run()
        {
            running = true;
            startT = EditorApplication.timeSinceStartup;
            nextProvoke = startT + 12.0;
            apexFired = false;
            AmbientMenu.SetAll(true);
        }

        [MenuItem("Ambient/Stop Show", false, 4)]
        static void Stop()
        {
            running = false;
            AmbientMenu.SetAll(false);
        }

        static void Tick()
        {
            if (!running)
            {
                return;
            }
            double now = EditorApplication.timeSinceStartup;
            double e = now - startT;
            if (e >= 128.0)
            {
                Stop();
                return;
            }

            if (e >= 116.0 && !apexFired)
            {
                apexFired = true;
                Director.Provoke(Cryptic.Pick(), 1f, true);
                nextProvoke = now + 2.5;
                return;
            }

            if (now < nextProvoke)
            {
                return;
            }

            float minGap, maxGap, minStr, maxStr;
            if (e < 25.0) { minGap = 18f; maxGap = 26f; minStr = 0.2f; maxStr = 0.3f; }
            else if (e < 60.0) { minGap = 14f; maxGap = 22f; minStr = 0.28f; maxStr = 0.45f; }
            else if (e < 95.0) { minGap = 9f; maxGap = 15f; minStr = 0.45f; maxStr = 0.65f; }
            else if (e < 115.0) { minGap = 6f; maxGap = 9f; minStr = 0.6f; maxStr = 0.85f; }
            else { minGap = 2.2f; maxGap = 3.2f; minStr = 0.9f; maxStr = 1f; }

            bool takeover = e >= 115.0 && Random.value < 0.5f;
            Director.Provoke(Cryptic.Pick(), Random.Range(minStr, maxStr), takeover);
            nextProvoke = now + Random.Range(minGap, maxGap);
        }
    }
}
