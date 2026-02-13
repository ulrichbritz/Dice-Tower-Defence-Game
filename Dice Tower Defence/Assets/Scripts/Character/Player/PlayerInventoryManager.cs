using UnityEngine;

namespace UB
{
    public class PlayerInventoryManager : CharacterInventoryManager
    {
        public DieItem CurrentDieHead;

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
