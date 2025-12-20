using UB;
using UnityEngine;

namespace UB
{
    public class IdleState : State
    {
        public override State RunCurrentState()
        {
            return this;
        }
    }
}
