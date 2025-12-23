using UnityEngine;
using UnityEngine.AI;

namespace UB
{
    public class AICharacterManager : CharacterManager
    {
        [Header("Internal Components")]
        // Internal Component Scripts
        [HideInInspector] public AILocomotionManager AILocomotionManager { get; private set; }
        [HideInInspector] public AIStatsManager AIStatsManager { get; private set; }
        // Internal Components
        [HideInInspector] public NavMeshAgent NavMeshAgent { get; private set; }

        [Header("AI Character Flags")]
        public bool HasTarget => TargetTransform != null;

        [Header("Target")]
        public Transform TargetTransform { get; private set; }

        [Header("Current State")]
        public AIState CurrentState { get; private set; }
        [SerializeField] private AIIdleState idleState;
        public AIIdleState IdleState => idleState;
        [SerializeField] private AIPursueTargetState pursueTargetState;
        public AIPursueTargetState PursueTargetState => pursueTargetState;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            // Internal Component Scripts
            AILocomotionManager = GetComponent<AILocomotionManager>();
            AIStatsManager = GetComponent<AIStatsManager>();

            // Internal Components
            NavMeshAgent = GetComponentInChildren<NavMeshAgent>();

            // Ensure the character is properly placed on the NavMesh
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas)) {
                // Disable the agent before positioning to prevent conflicts
                NavMeshAgent.enabled = false;
                transform.position = hit.position;

                // Re-enable the agent and ensure it's properly initialized
                NavMeshAgent.enabled = true;
            } else {
                Debug.LogError($"Could not find NavMesh position near {transform.position}");
            }

            // Use a copy of the SO so the original is not modified
            idleState = Instantiate(idleState);
            pursueTargetState = Instantiate(pursueTargetState);

            CurrentState = IdleState;
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            ProcessStateMachine();
        }

        private void ProcessStateMachine()
        {
            AIState nextState = CurrentState?.Tick(this);

            if (nextState != null) {
                CurrentState = nextState;
            }
        }

        public void AssignTarget()
        {
            TargetTransform = PlayerManager.Instance.transform;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Internal References Cleanup
            NavMeshAgent = null;
        }
    }
}
