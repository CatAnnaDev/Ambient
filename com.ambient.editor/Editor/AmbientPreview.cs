using UnityEditor;
using UnityEngine;

namespace Ambient
{
    public class AmbientPreview : EditorWindow
    {
        WebCamTexture cam;

        public static void Open()
        {
            var window = GetWindow<AmbientPreview>(false, "Preview");
            window.minSize = new Vector2(240f, 180f);
            window.Show();
        }

        public static void CloseIfOpen()
        {
            if (HasOpenInstances<AmbientPreview>())
            {
                GetWindow<AmbientPreview>().Close();
            }
        }

        void OnEnable()
        {
            try
            {
                cam = new WebCamTexture();
                cam.Play();
            }
            catch
            {
                cam = null;
            }
        }

        void OnDisable()
        {
            if (cam != null)
            {
                cam.Stop();
                cam = null;
            }
        }

        void OnGUI()
        {
            var frame = new Rect(0f, 0f, position.width, position.height);
            if (cam != null && cam.width > 16)
            {
                GUI.DrawTexture(frame, cam, ScaleMode.ScaleAndCrop, false);
            }
            else
            {
                EditorGUI.DrawRect(frame, new Color(0.02f, 0.02f, 0.02f, 1f));
            }
            Repaint();
        }
    }
}
