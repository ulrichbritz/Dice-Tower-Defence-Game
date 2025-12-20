using UnityEngine;

namespace UB
{
    public class EnemyAttackState : AttackState
    {
        public override State RunCurrentState()
        {
            return this;
        }
    }
}
