using AsyncRoutines;
using UnityEngine;

namespace UB
{
    public class CharacterCombatManager : MonoBehaviour
    {
        [Header("Weapon Management")]
        public WeaponItem CurrentWeaponBeingUsed;

        [Header("Attack Cooldown")]
        public bool AttackOffCooldown => AttackCooldownTimer <= 0f;
        public float AttackCooldownTimer { get; private set; }

        protected virtual void Awake()
        {
            // TODO: move this later to when we add multiple weapons
            CurrentWeaponBeingUsed = GetComponent<CharacterInventoryManager>().CurrentRightHandWeapon;
        }

        protected virtual void Update()
        {
            // Todo: remove this check when we set up enemy weapons.
            if (CurrentWeaponBeingUsed == null) {
                return;
            }

            // Update attack cooldown timer
            if (AttackCooldownTimer > 0f) {
                AttackCooldownTimer -= Time.deltaTime;
                if (AttackCooldownTimer < 0f) {
                    AttackCooldownTimer = 0f;
                }
            }
        }

        protected virtual void AttemptToPerformAttack()
        {
            if (AttackOffCooldown) {
                WorldRoutineManager.Instance.Run(PerformAttack());
            }
        }

        protected virtual async Routine PerformAttack()
        {
            // Start cooldown
            if (CurrentWeaponBeingUsed != null) {
                AttackCooldownTimer = 1f / CurrentWeaponBeingUsed.CurrentAttackSpeed;
            }
        }


        protected virtual void OnDestroy()
        {

        }
    }
}


