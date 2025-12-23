using UnityEngine;

namespace UB
{
    public class Item : ScriptableObject
    {
        [Header("Item Information")]
        public string ItemName;
        public Sprite ItemIcon;
        [TextArea]
        public string ItemDescription;
        public int ItemID;
    }
}

