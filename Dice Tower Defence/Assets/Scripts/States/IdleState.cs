using UnityEngine;

namespace UB
{
    public class IdleState : State
    {
        public ChaseState ChaseState;
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
