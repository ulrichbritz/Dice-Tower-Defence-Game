using Unity.VisualScripting;
using UnityEngine;

namespace UB
{

    public class EnemyLocomotionManager : CharacterLocomotionManager
    {
        [Header("Components")]
        [HideInInspector] public EnemyManager EnemyManager;

        [Header("Movement Settings")]
        private Transform BarricadeTarget;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            EnemyManager = GetComponent<EnemyManager>();

            // Set NavMeshAgent speed based on CharacterStats
            EnemyManager.NavMeshAgent.speed = EnemyManager.CharacterStatsManager.MovementSpeed;
            EnemyManager.NavMeshAgent.acceleration = EnemyManager.CharacterStatsManager.Acceleration;
            EnemyManager.NavMeshAgent.stoppingDistance = EnemyManager.CharacterStatsManager.StoppingDistance;
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
        }

        protected override void HandleMovement()
        {

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            EnemyManager = null;
            BarricadeTarget = null;
        }
    }
}
