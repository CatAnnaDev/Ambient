using UnityEditor;
using UnityEngine;

namespace Ambient
{
    public class FallingPanel : EditorWindow
    {
        public string label;

        void OnGUI()
        {
            var full = new Rect(0f, 0f, position.width, position.height);
            EditorGUI.DrawRect(full, new Color(0.13f, 0.13f, 0.14f, 1f));
            EditorGUI.DrawRect(new Rect(0f, 0f, position.width, 20f), new Color(0.07f, 0.07f, 0.08f, 1f));

            var st = new GUIStyle(EditorStyles.boldLabel);
            st.normal.textColor = new Color(0.6f, 0.6f, 0.62f);
            GUI.Label(new Rect(6f, 2f, position.width - 12f, 18f), label ?? "", st);

            EditorGUI.DrawRect(new Rect(0f, 20f, position.width, 1f), new Color(0.5f, 0.05f, 0.05f, 0.5f));
        }
    }
}
