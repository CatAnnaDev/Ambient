using UnityEngine;

namespace Ambient
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Ambient/Marker")]
    public class Marker : MonoBehaviour
    {
        public float weight = 1f;
        public Vector3 offset;
        public bool active = true;
        public string label = "";
    }
}
