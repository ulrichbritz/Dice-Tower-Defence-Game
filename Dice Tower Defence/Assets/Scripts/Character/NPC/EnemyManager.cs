using UnityEngine;
using UnityEngine.AI;

namespace UB
{
    public class EnemyManager : CharacterManager
    {
        [Header("Enemy Components")]
        [HideInInspector] public NavMeshAgent NavMeshAgent { get; private set; }
        [HideInInspector] public EnemyStateManager EnemyStateManager { get; private set; }
        
        public override void Awake()
        {

        }

        public override void Start()
        {
            base.Start();

            NavMeshAgent = GetComponent<NavMeshAgent>();
            EnemyStateManager = GetComponent<EnemyStateManager>();
        }

        public override void Update()
        {

        }

        public override void FixedUpdate()
        {

        }

        public override void LateUpdate()
        {

        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            // Internal References Cleanup
            NavMeshAgent = null;
        }
    }
}
