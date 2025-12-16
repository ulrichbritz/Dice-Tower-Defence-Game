using UnityEngine;

namespace UB
{
    public class ChaseState : State
    {
        public AttackState AttackState;
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
