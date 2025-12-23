using UnityEngine;

namespace UB
{
    [CreateAssetMenu(menuName ="Items/Die Item")]
    public class DieItem : Item
    {
        public DieFace[] DieFaces;

        public GameObject DieModel;
    }
}

