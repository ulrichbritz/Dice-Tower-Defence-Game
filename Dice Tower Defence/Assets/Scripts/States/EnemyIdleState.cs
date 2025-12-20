using UnityEngine;

namespace UB
{
    public class EnemyIdleState : IdleState
    {
        public EnemyChaseState ChaseState;
        public bool ReadyToMove;

        public override State RunCurrentState()
        {
            if (ReadyToMove) {
                return ChaseState;
            }

            return this;
        }
    }
}
