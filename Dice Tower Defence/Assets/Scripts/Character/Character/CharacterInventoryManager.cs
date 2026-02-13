using UnityEngine;

namespace UB
{
    public class CharacterInventoryManager : MonoBehaviour
    {
        public WeaponItem CurrentRightHandWeapon;   // Will be the main weapon that we base the stats off etc
        public WeaponItem CurrentLeftHandWeapon;

        protected virtual void Awake()
        {
        }

        protected virtual void Update()
        {
        }

        protected virtual void OnDestroy()
        {

        }

    }
}
