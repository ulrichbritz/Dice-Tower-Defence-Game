using UnityEngine;

namespace UB
{
    public abstract class StateManager : MonoBehaviour
    {
        public State CurrentState;

        protected virtual void Update()
        {
            RunStateMachine();
        }

        protected virtual void RunStateMachine()
        {
            State nextState = CurrentState?.RunCurrentState();

            if (nextState != null) {
                // Switch to the next state
                SwitchToNextState(nextState);
            }
        }

        public virtual void SwitchToNextState(State nextState)
        {
            CurrentState = nextState;
        }
    }
}
