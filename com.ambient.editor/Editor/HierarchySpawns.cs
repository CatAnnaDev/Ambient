using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Ambient
{
    [InitializeOnLoad]
    static class HierarchySpawns
    {
        static readonly char[] Marks =
        {
            '\u0300', '\u0301', '\u0302', '\u0303', '\u0308', '\u030A', '\u0323', '\u0489',
        };

        static readonly string[] Bases =
        {
            "ENTITY", "obj", "void", "no_name", "0x6e6f", "IT", "host", "null_", "hollow", "witness",
        };

        class Spawned
        {
            public GameObject go;
            public double until;
        }

        static readonly List<Spawned> live = new List<Spawned>();

        static HierarchySpawns()
        {
            Director.OnSurge += OnSurge;
            EditorApplication.update += Tick;
            SceneView.duringSceneGui += OnScene;
        }

        static void OnSurge()
        {
            if (!AmbientMenu.IsOn(AmbientMenu.Spawns) || Random.value > 0.5f)
            {
                return;
            }
            Spawn();
        }

        static void Spawn()
        {
            var go = new GameObject(CrypticName());
            go.hideFlags = HideFlags.DontSaveInEditor;
            go.transform.position = new Vector3(Random.Range(-3f, 3f), Random.Range(-1f, 3f), Random.Range(-3f, 3f));
            if (Random.value < 0.6f)
            {
                AttachBroken(go);
            }
            live.Add(new Spawned { go = go, until = EditorApplication.timeSinceStartup + Random.Range(4f, 9f) });
            EditorApplication.RepaintHierarchyWindow();
        }

        static void AttachBroken(GameObject go)
        {
            try
            {
                var seed = go.AddComponent<Marker>();
                var so = new SerializedObject(seed);
                var sp = so.FindProperty("m_Script");
                if (sp != null)
                {
                    sp.objectReferenceValue = null;
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
            }
            catch
            {
            }
        }

        static void Tick()
        {
            if (live.Count == 0)
            {
                return;
            }
            double t = EditorApplication.timeSinceStartup;
            bool on = AmbientMenu.IsOn(AmbientMenu.Spawns);
            for (int i = live.Count - 1; i >= 0; i--)
            {
                var s = live[i];
                if (s.go == null)
                {
                    live.RemoveAt(i);
                    continue;
                }
                if (on && Random.value < 0.015f)
                {
                    s.go.name = CrypticName();
                    EditorApplication.RepaintHierarchyWindow();
                }
                if (!on || t >= s.until)
                {
                    Object.DestroyImmediate(s.go);
                    live.RemoveAt(i);
                }
            }
        }

        static void OnScene(SceneView view)
        {
            if (!AmbientMenu.IsOn(AmbientMenu.Spawns) || live.Count == 0)
            {
                return;
            }
            var cam = view.camera;
            if (cam == null)
            {
                return;
            }
            var pc = Handles.color;
            for (int i = 0; i < live.Count; i++)
            {
                var s = live[i];
                if (s.go == null)
                {
                    continue;
                }
                Vector3 pos = s.go.transform.position;
                float sz = HandleUtility.GetHandleSize(pos) * 0.06f;
                Vector3 right = cam.transform.right;
                Vector3 up = cam.transform.up;
                Vector3 fwd = (cam.transform.position - pos).normalized;
                Vector3 head = pos + up * sz * 3f;
                Handles.color = new Color(0.85f, 0.05f, 0.05f, 0.9f);
                Handles.DrawSolidDisc(head - right * sz * 1.4f, fwd, sz);
                Handles.DrawSolidDisc(head + right * sz * 1.4f, fwd, sz);
            }
            Handles.color = pc;
            view.Repaint();
        }

        static string CrypticName()
        {
            string b = Bases[Random.Range(0, Bases.Length)];
            var sb = new StringBuilder(b.Length * 3);
            foreach (char c in b)
            {
                sb.Append(c);
                int n = Random.Range(0, 3);
                for (int i = 0; i < n; i++)
                {
                    sb.Append(Marks[Random.Range(0, Marks.Length)]);
                }
            }
            return sb.ToString();
        }
    }
}
