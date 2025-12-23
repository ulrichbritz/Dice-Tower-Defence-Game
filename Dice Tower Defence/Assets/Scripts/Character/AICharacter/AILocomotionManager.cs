using Unity.VisualScripting;
using UnityEngine;

namespace UB
{

    public class AILocomotionManager : CharacterLocomotionManager
    {
        [Header("Components")]
        [HideInInspector] public AICharacterManager AICharacterManager;

        [Header("Movement Settings")]
        private Transform BarricadeTarget;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            AICharacterManager = GetComponent<AICharacterManager>();

            // Set NavMeshAgent speed based on CharacterStats
            AICharacterManager.NavMeshAgent.speed = AICharacterManager.AIStatsManager.CurrentCharacterStats.MovementSpeed;
            //AICharacterManager.NavMeshAgent.acceleration = AICharacterManager.AIStatsManager.CurrentCharacterStats.Acceleration;
            AICharacterManager.NavMeshAgent.stoppingDistance = AICharacterManager.AIStatsManager.CurrentCharacterStats.StoppingDistance;
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

            AICharacterManager = null;
            BarricadeTarget = null;
        }
    }
}
