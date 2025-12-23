using UnityEngine;

namespace UB
{
    [CreateAssetMenu(menuName ="AI/States/Idle State")]
    public class AIIdleState : AIState
    {
        public override AIState Tick(AICharacterManager aiCharacter)
        {
            // If Wave is in progress, change to the pursue state
            if (GameManager.Instance.WaveCurrentlyInProgress && aiCharacter.HasTarget) {
                return SwitchState(aiCharacter, aiCharacter.PursueTargetState);
            }
            else if (!GameManager.Instance.WaveCurrentlyInProgress) {
                // if wave hasnt started, stay in idle state
            }
            else {
                // Assign target if none exists
                aiCharacter.AssignTarget();
            }

            return this;
        }
    }
}
