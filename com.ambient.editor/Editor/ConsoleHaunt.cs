using System.Text;
using UnityEditor;
using UnityEngine;

namespace Ambient
{
    [InitializeOnLoad]
    static class ConsoleHaunt
    {
        static ConsoleHaunt()
        {
            Director.OnSurge += OnManifest;
        }

        static void OnManifest()
        {
            if (!AmbientMenu.IsOn(AmbientMenu.Console))
            {
                return;
            }
            if (UnityEngine.Random.value >= 0.25f)
            {
                return;
            }
            AmbientLog.Say("Assets/" + Token() + ".cs(1,1): " + Cryptic.Any(Cryptic.Errors), LogType.Error);
        }

        static string Token()
        {
            const string c = "abcdefghijklmnopqrstuvwxyz";
            var sb = new StringBuilder(6);
            for (int i = 0; i < 6; i++)
            {
                sb.Append(c[UnityEngine.Random.Range(0, c.Length)]);
            }
            return sb.ToString();
        }
    }
}
