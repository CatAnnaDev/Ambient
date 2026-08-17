using UnityEditor;
using UnityEngine;

namespace Ambient
{
    [CustomEditor(typeof(Marker))]
    public class MarkerInspector : Editor
    {
        static readonly string[] Certainties =
        {
            "vrai et faux",
            "ni l'un ni l'autre",
            "ca depend de qui regarde",
            "pas encore decide",
        };

        public override bool RequiresConstantRepaint() => true;

        public override void OnInspectorGUI()
        {
            var red = new GUIStyle(EditorStyles.boldLabel);
            red.normal.textColor = new Color(0.86f, 0.12f, 0.12f);

            double t = EditorApplication.timeSinceStartup;
            float draining = Mathf.Repeat((float)(-t * 6.0), 100f);
            var impossible = new Vector3(Mathf.Sin((float)t) * 99f, float.NaN, -0f);

            EditorGUILayout.LabelField(
                "position reelle",
                string.Format("({0:0.###}, {1}, {2})", impossible.x, impossible.y, impossible.z),
                red
            );
            EditorGUILayout.LabelField(
                "certitude",
                Certainties[(int)(t * 0.7) % Certainties.Length],
                red
            );
            EditorGUILayout.LabelField("temps restant", (-1).ToString(), red);
            EditorGUILayout.LabelField("observe", "oui", red);

            DrawDefaultInspector();
        }
    }
}
