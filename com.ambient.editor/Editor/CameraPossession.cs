using UnityEditor;
using UnityEngine;

namespace Ambient
{
    [InitializeOnLoad]
    static class CameraPossession
    {
        static SceneView sv;
        static bool active;
        static bool userAbort;
        static double start;
        static float dur;
        static int mode;

        static Quaternion savedRot;
        static Vector3 savedPiv;
        static float savedSize;
        static Quaternion toRot;
        static Vector3 toPiv;
        static float toSize;
        static float pitch;

        static CameraPossession()
        {
            Director.OnSurge += OnSurge;
            EditorApplication.update += Tick;
            SceneView.duringSceneGui += OnScene;
        }

        static void OnSurge()
        {
            if (!AmbientMenu.IsOn(AmbientMenu.Presence) || active)
            {
                return;
            }
            var v = SceneView.lastActiveSceneView;
            if (v == null)
            {
                return;
            }
            if (!Director.IsTakeover && Random.value > Director.SurgeStrength)
            {
                return;
            }
            sv = v;
            start = EditorApplication.timeSinceStartup;
            dur = Director.IsTakeover ? 2.4f : Random.Range(1.4f, 2.6f);
            savedRot = sv.rotation;
            savedPiv = sv.pivot;
            savedSize = sv.size;
            userAbort = false;
            PickMove();
            active = true;
        }

        static void PickMove()
        {
            mode = Director.IsTakeover
                ? (Random.value < 0.5f ? 4 : 5)
                : Random.Range(0, 6);

            toRot = savedRot;
            toPiv = savedPiv;
            toSize = savedSize;
            pitch = Random.Range(-12f, 12f);
            float sign = Random.value < 0.5f ? -1f : 1f;

            switch (mode)
            {
                case 0:
                    toRot = savedRot * Quaternion.Euler(Random.Range(-15f, 15f), Random.Range(80f, 160f) * sign, 0f);
                    break;
                case 1:
                    var tgt = RandomSceneObject();
                    if (tgt != null)
                    {
                        toPiv = tgt.transform.position;
                        toSize = savedSize * Random.Range(0.3f, 0.6f);
                        toRot = savedRot * Quaternion.Euler(Random.Range(-20f, 20f), Random.Range(-70f, 70f), 0f);
                    }
                    else
                    {
                        toRot = savedRot * Quaternion.Euler(0f, 100f * sign, 0f);
                    }
                    break;
                case 2:
                    toSize = savedSize * Random.Range(0.25f, 0.5f);
                    break;
                case 3:
                    toRot = savedRot * Quaternion.Euler(Random.Range(-10f, 10f), Random.Range(-25f, 25f), 0f);
                    toPiv = savedPiv + savedRot * new Vector3(Random.Range(-2f, 2f), Random.Range(-1f, 1f), 0f) * (savedSize * 0.3f);
                    toSize = savedSize * Random.Range(0.8f, 1.2f);
                    break;
            }
        }

        static void Tick()
        {
            if (!active)
            {
                return;
            }
            if (sv == null || !AmbientMenu.IsOn(AmbientMenu.Presence) || userAbort)
            {
                End(sv != null && !userAbort);
                return;
            }
            double t = EditorApplication.timeSinceStartup;
            float p = dur > 0f ? Mathf.Clamp01((float)((t - start) / dur)) : 1f;

            if (mode == 4)
            {
                float amp = 9f * Mathf.Sin(p * Mathf.PI) * Director.SurgeStrength;
                sv.rotation = savedRot * Quaternion.Euler(Mathf.Sin((float)t * 45f) * amp, Mathf.Cos((float)t * 38f) * amp, 0f);
                sv.size = savedSize * (1f - 0.3f * Mathf.Sin(p * Mathf.PI) * Mathf.Abs(Mathf.Sin((float)t * 20f)));
            }
            else if (mode == 5)
            {
                sv.rotation = savedRot * Quaternion.Euler(pitch * Mathf.Sin(p * Mathf.PI), p * 360f, 0f);
            }
            else
            {
                float e = Mathf.Sin(p * Mathf.PI);
                sv.rotation = Quaternion.Slerp(savedRot, toRot, e);
                sv.pivot = Vector3.Lerp(savedPiv, toPiv, e);
                sv.size = Mathf.Lerp(savedSize, toSize, e);
            }

            sv.Repaint();
            if (p >= 1f)
            {
                End(true);
            }
        }

        static void OnScene(SceneView v)
        {
            if (!active || v != sv)
            {
                return;
            }
            var e = Event.current;
            if (e.type == EventType.MouseDrag || e.type == EventType.ScrollWheel || e.type == EventType.KeyDown)
            {
                userAbort = true;
            }
        }

        static void End(bool restore)
        {
            if (sv != null && restore)
            {
                try
                {
                    sv.rotation = savedRot;
                    sv.pivot = savedPiv;
                    sv.size = savedSize;
                    sv.Repaint();
                }
                catch
                {
                }
            }
            active = false;
            sv = null;
            userAbort = false;
        }

        static GameObject RandomSceneObject()
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                return null;
            }
            var roots = scene.GetRootGameObjects();
            if (roots.Length == 0)
            {
                return null;
            }
            return roots[Random.Range(0, roots.Length)];
        }
    }
}
