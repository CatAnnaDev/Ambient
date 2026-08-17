using UnityEditor;
using UnityEngine;

namespace Ambient
{
    [InitializeOnLoad]
    static class SceneAware
    {
        static SceneAware()
        {
            SceneView.duringSceneGui += OnScene;
        }

        static void OnScene(SceneView view)
        {
            if (!AmbientMenu.IsOn(AmbientMenu.Presence) || !Director.Surging)
            {
                return;
            }
            float env = Director.Envelope() * Director.SurgeStrength;
            if (env < 0.03f)
            {
                return;
            }

            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            GameObject target = null;
            if (scene.IsValid())
            {
                var roots = scene.GetRootGameObjects();
                if (roots.Length > 0)
                {
                    int seed = Mathf.Abs((Director.SurgeMessage ?? "x").GetHashCode());
                    target = roots[seed % roots.Length];
                }
            }
            if (target == null)
            {
                return;
            }

            Vector3 pos = target.transform.position;
            DrawEyes(view, pos, env);

            var style = new GUIStyle(EditorStyles.boldLabel);
            style.normal.textColor = new Color(0.85f, 0.06f, 0.06f, env);
            Handles.Label(pos + Vector3.up * 0.6f, Director.SurgeMessage ?? "", style);

            view.Repaint();
        }

        static void DrawEyes(SceneView view, Vector3 pos, float env)
        {
            var cam = view.camera;
            if (cam == null)
            {
                return;
            }
            float sz = HandleUtility.GetHandleSize(pos) * 0.06f;
            Vector3 right = cam.transform.right;
            Vector3 up = cam.transform.up;
            Vector3 fwd = (cam.transform.position - pos).normalized;
            Vector3 head = pos + up * sz * 3f;

            var prev = Handles.color;
            Handles.color = new Color(0.85f, 0.05f, 0.05f, env);
            Handles.DrawSolidDisc(head - right * sz * 1.4f, fwd, sz);
            Handles.DrawSolidDisc(head + right * sz * 1.4f, fwd, sz);
            Handles.color = prev;
        }
    }
}
