using UnityEngine;

namespace UB
{
    public class EnemyChaseState : ChaseState
    {
        public EnemyAttackState AttackState;
        public bool IsInAttackRange;

        public override State RunCurrentState()
        {
            if (IsInAttackRange) {
                return AttackState;
            }
            else {
                return this;
            }
        }
    }
}
