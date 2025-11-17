using UnityEngine;

namespace UB
{
    public class Die : ScriptableObject
    {
        public string DieName;
        public string Description;
        public DieFace[] DieFaces;

        public GameObject Model;
    }
}

