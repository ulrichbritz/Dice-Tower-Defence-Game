using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using AsyncRoutines;

namespace UB
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        [Header("Player Combat")]
        private PlayerManager playerManager;
        private Transform currentTarget;

        [Header("Rotation")]
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private bool smoothRotation = true;

        [Header("Debug")]
        [SerializeField] private bool showAttackRange = true;
        [SerializeField] private bool showDebugInfo = true;

        protected override void Awake()
        {
            base.Awake();

            playerManager = GetComponent<PlayerManager>();
        }

        protected override void Update()
        {
            base.Update();

            // Auto-target and attack enemies when wave is active
            HandleAutoTargeting();
        }

        private void HandleAutoTargeting()
        {
            // Only auto-target during active waves
            if (!GameManager.Instance.WaveCurrentlyInProgress) {
                currentTarget = null;
                return;
            }

            // Only attack if we have a weapon and are off cooldown
            if (CurrentWeaponBeingUsed == null || !AttackOffCooldown) {
                return;
            }

            // Find closest enemy in range
            var closestEnemy = FindClosestEnemyInRange();

            if (closestEnemy != null) {
                currentTarget = closestEnemy;

                // Face the target (smooth or instant)
                FaceTarget(currentTarget);

                // Attack!
                AttemptToPerformAttack();
            }
            else {
                currentTarget = null;
            }
        }

        private void FaceTarget(Transform target)
        {
            if (target == null) return;

            Vector3 directionToTarget = (target.position - transform.position).normalized;
            directionToTarget.y = 0f; // Keep on horizontal plane

            if (directionToTarget != Vector3.zero) {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

                if (smoothRotation) {
                    // Smooth rotation over time
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
                else {
                    // Instant rotation
                    transform.rotation = targetRotation;
                }
            }
        }

        private Transform FindClosestEnemyInRange()
        {
            if (CurrentWeaponBeingUsed == null) {
                return null;
            }

            float attackRange = CurrentWeaponBeingUsed.CurrentAttackRange;
            Transform closestEnemyTransform = null;
            float closestDistance = float.MaxValue;

            // Get all spawned enemies from WorldAIManager
            if (WorldAIManager.Instance?.SpawnedCharacters != null) {
                foreach (var enemyGameObject in WorldAIManager.Instance.SpawnedCharacters) {
                    if (enemyGameObject == null) continue;

                    // Check if enemy is still alive
                    CharacterStatsManager enemyStats = enemyGameObject.GetComponent<CharacterStatsManager>();
                    if (enemyStats != null && enemyStats.IsDead) continue;

                    float distance = Vector3.Distance(transform.position, enemyGameObject.transform.position);

                    if (distance <= attackRange && distance < closestDistance) {
                        closestDistance = distance;
                        closestEnemyTransform = enemyGameObject.transform;
                    }
                }
            }

            return closestEnemyTransform;
        }

        protected override void AttemptToPerformAttack()
        {
            base.AttemptToPerformAttack();
        }

        protected override async Routine PerformAttack()
        {
            await base.PerformAttack();

            Debug.Log("PerformAttack called!");

            if (playerManager.IsPerformingAction) {
                Debug.Log("Player is already performing action, skipping attack");
                return;
            }

            if (CurrentWeaponBeingUsed?.AttackAnimations == null || CurrentWeaponBeingUsed.AttackAnimations.Count == 0) {
                Debug.LogError("No attack animations available on weapon!");
                return;
            }

            var dieController = playerManager.PlayerEquipmentManager.DieController;

            // Use the new async method to get the die result after animation
            int dieResult = await dieController.RollDieAndGetResult();

            Debug.Log($"Die rolled: {dieResult}!");

            // Calculate total damage: weapon base * die roll
            float totalDamage = (CurrentWeaponBeingUsed?.CurrentPhysicalDamage ?? 0f) * dieResult;

            Debug.Log($"Total damage: {totalDamage} (weapon: {CurrentWeaponBeingUsed?.CurrentPhysicalDamage ?? 0f} * die: {dieResult})");

            Debug.Log($"Playing attack animation from weapon with {CurrentWeaponBeingUsed.AttackAnimations.Count} animations");

            var animation = CurrentWeaponBeingUsed.AttackAnimations[Random.Range(0, CurrentWeaponBeingUsed.AttackAnimations.Count)];

            playerManager.PlayerAnimatorManager.PlayTargetActionAnimation(
                targetAnimation: animation.name,
                isPerformingAction: true,
                applyRootMotion:false,
                canMove: true,
                canRotate: false
                );

            // Apply damage to current target
            if (currentTarget != null) {
                DealDamageToTarget(currentTarget, totalDamage);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        /// <summary>
        /// Deal damage to a target
        /// </summary>
        private void DealDamageToTarget(Transform target, float damage)
        {
            if (target == null) return;

            CharacterStatsManager targetStats = target.GetComponent<CharacterStatsManager>();
            if (targetStats != null && !targetStats.IsDead) {
                int damageAmount = Mathf.RoundToInt(damage);
                targetStats.CurrentCharacterStats.CurrentHealth -= damageAmount;

                Debug.Log($"Dealt {damageAmount} damage to {target.name}! Remaining health: {targetStats.CurrentCharacterStats.CurrentHealth}");
            }
        }

        // Debug visualization - always visible
        private void OnDrawGizmos()
        {
            if (!showAttackRange || CurrentWeaponBeingUsed == null) return;

            // Draw attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, CurrentWeaponBeingUsed.CurrentAttackRange);

            // Draw line to current target
            if (currentTarget != null) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, currentTarget.position);
            }
        }

        // Debug visualization - only when selected
        private void OnDrawGizmosSelected()
        {
            if (!showAttackRange || CurrentWeaponBeingUsed == null) return;

            // Draw attack range with different color when selected
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, CurrentWeaponBeingUsed.CurrentAttackRange);

            // Draw line to current target
            if (currentTarget != null) {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, currentTarget.position);
            }
        }

        private void OnGUI()
        {
            if (!showDebugInfo) return;

            GUI.Label(new Rect(10, 10, 400, 20), $"Wave Active: {GameManager.Instance.WaveCurrentlyInProgress}");
            GUI.Label(new Rect(10, 30, 400, 20), $"Weapon: {(CurrentWeaponBeingUsed != null ? CurrentWeaponBeingUsed.ItemName : "None")}");
            GUI.Label(new Rect(10, 50, 400, 20), $"Attack Cooldown: {AttackCooldownTimer:F2}s (Ready: {AttackOffCooldown})");
            GUI.Label(new Rect(10, 70, 400, 20), $"Current Target: {(currentTarget != null ? currentTarget.name : "None")}");
            GUI.Label(new Rect(10, 90, 400, 20), $"Player Performing Action: {playerManager.IsPerformingAction}");
            if (CurrentWeaponBeingUsed != null) {
                GUI.Label(new Rect(10, 110, 400, 20), $"Weapon Range: {CurrentWeaponBeingUsed.CurrentAttackRange}");
                GUI.Label(new Rect(10, 130, 400, 20), $"Attack Speed: {CurrentWeaponBeingUsed.CurrentAttackSpeed}");
                GUI.Label(new Rect(10, 150, 400, 20), $"Animations: {CurrentWeaponBeingUsed.AttackAnimations?.Count ?? 0}");
            }
        }
    }
}

