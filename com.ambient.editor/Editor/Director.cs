using System;
using UnityEditor;
using UnityEngine;

namespace Ambient
{
    [InitializeOnLoad]
    static class Director
    {
        public static bool Surging { get; private set; }
        public static float SurgeStrength { get; private set; }
        public static string SurgeMessage { get; private set; }
        public static bool IsTakeover { get; private set; }

        static double surgeStart;
        static float surgeDuration;
        static double nextSurge;

        public static event Action OnSurge;

        static Director()
        {
            EditorApplication.update += Tick;
        }

        static double Now => EditorApplication.timeSinceStartup;

        public static float Envelope()
        {
            if (!Surging)
            {
                return 0f;
            }
            float p = surgeDuration > 0f ? Mathf.Clamp01((float)((Now - surgeStart) / surgeDuration)) : 1f;
            return p < 0.15f ? p / 0.15f : 1f - (p - 0.15f) / 0.85f;
        }

        public static void Provoke(string message, float strength)
        {
            Begin(Now, message, Mathf.Clamp01(strength), false);
            nextSurge = Now + Interval();
        }

        public static void Provoke(string message, float strength, bool takeover)
        {
            Begin(Now, message, Mathf.Clamp01(strength), takeover);
            nextSurge = Now + Interval();
        }

        static void Tick()
        {
            double t = Now;
            if (Surging && t - surgeStart >= surgeDuration)
            {
                Surging = false;
            }
            if (!AnyEnabled())
            {
                nextSurge = 0.0;
                return;
            }
            if (nextSurge <= 0.0)
            {
                nextSurge = t + Interval();
                return;
            }
            if (t >= nextSurge && !Surging)
            {
                float strength = Mathf.Clamp01(0.4f + Escalation.Intensity * 0.6f + UnityEngine.Random.value * 0.15f);
                Begin(t, Cryptic.Pick(), strength, false);
                nextSurge = t + Interval();
            }
        }

        static void Begin(double t, string message, float strength, bool forceTakeover)
        {
            Surging = true;
            surgeStart = t;
            SurgeStrength = strength;
            IsTakeover = forceTakeover || (strength > 0.8f && UnityEngine.Random.value < (0.12f + Escalation.Intensity * 0.4f));
            surgeDuration = IsTakeover ? UnityEngine.Random.Range(2.2f, 3.0f) : UnityEngine.Random.Range(1.6f, 3.2f);
            SurgeMessage = message;
            var handler = OnSurge;
            if (handler != null)
            {
                handler();
            }
        }

        static float Interval()
        {
            float inten = Escalation.Intensity;
            float min = Mathf.Lerp(35f, 12f, inten);
            float max = Mathf.Lerp(90f, 30f, inten);
            return UnityEngine.Random.Range(min, max);
        }

        static bool AnyEnabled()
        {
            return AmbientMenu.IsOn(AmbientMenu.Whispers)
                || AmbientMenu.IsOn(AmbientMenu.HauntEditor)
                || AmbientMenu.IsOn(AmbientMenu.Screen)
                || AmbientMenu.IsOn(AmbientMenu.Presence)
                || AmbientMenu.IsOn(AmbientMenu.Reactive);
        }
    }
}
