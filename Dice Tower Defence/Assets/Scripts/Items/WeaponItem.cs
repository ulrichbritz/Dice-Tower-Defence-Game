using UnityEngine;

namespace UB
{
    public class WeaponItem : Item
    {
        // Animator controller for different animations

        [Header("Weapon Model")]
        public GameObject WeaponModel;

        [Header("Base Weapon Stats")]
        public float BasePhysicalDamage = 0f;   // extra damage added to die roll
        public float BaseAttackSpeed = 1f;  // attacks per second
        public float BaseCriticalChance = 0f; // percentage
        public float BaseCriticalDamageModifier = 50f; // percentage
        public float BaseMovementSpeedWithWeapon = 5f;  // in units
        public float BaseEvasionChanceWithWeapon = 0f; // percentage
        public float BaseAttackRange = 2f; // in units

        [Header("Current Weapon Stats")]
        public float CurrentPhysicalDamage = 0f;
        public float CurrentAttackSpeed = 1f;
        public float CurrentCriticalChance = 0f;
        public float CurrentMovementSpeedWithWeapon = 5f;
        public float CurrentEvasionChanceWithWeapon = 0f;
        public float CurrentAttackRange = 2f;
    }
}
