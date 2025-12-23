using UnityEngine;

namespace UB
{
    public class PlayerInventoryManager : CharacterInventoryManager
    {
        public WeaponItem CurrentRightHandWeapon;
        public WeaponItem CurrentLeftHandWeapon;

        public DieItem CurrentDieHead;

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
