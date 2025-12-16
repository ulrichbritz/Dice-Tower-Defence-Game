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

        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            base.Start();

            EnemyManager = GetComponent<EnemyManager>();

            // Set NavMeshAgent speed based on CharacterStats
            EnemyManager.NavMeshAgent.speed = EnemyManager.CharacterStatsManager.MovementSpeed;
            EnemyManager.NavMeshAgent.acceleration = EnemyManager.CharacterStatsManager.Acceleration;
            EnemyManager.NavMeshAgent.stoppingDistance = EnemyManager.CharacterStatsManager.StoppingDistance;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
        }

        public override void HandleMovement()
        {

        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            EnemyManager = null;
            BarricadeTarget = null;
        }
    }
}
