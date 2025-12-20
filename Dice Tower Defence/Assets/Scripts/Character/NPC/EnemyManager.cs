using UnityEngine;
using UnityEngine.AI;

namespace UB
{
    public class EnemyManager : CharacterManager
    {
        [Header("Enemy Components")]
        [HideInInspector] public NavMeshAgent NavMeshAgent { get; private set; }
        [HideInInspector] public EnemyStateManager EnemyStateManager { get; private set; }
        
        protected override void Awake()
        {

        }

        protected override void Start()
        {
            base.Start();

            NavMeshAgent = GetComponent<NavMeshAgent>();
            EnemyStateManager = GetComponent<EnemyStateManager>();
        }

        protected override void Update()
        {

        }

        protected override void FixedUpdate()
        {

        }

        protected override void LateUpdate()
        {

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Internal References Cleanup
            NavMeshAgent = null;
        }
    }
}
