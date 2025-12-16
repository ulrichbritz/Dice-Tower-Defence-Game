using UnityEngine;

namespace UB
{
    public class AttackState : State
    {
        public override State RunCurrentState()
        {
            return this;
        }
    }
}
