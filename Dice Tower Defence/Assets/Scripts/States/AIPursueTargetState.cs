using UnityEngine;
using UnityEngine.AI;

namespace UB
{
    [CreateAssetMenu(menuName ="AI/States/Pursue Target State")]
    public class AIPursueTargetState : AIState
    {
        public override AIState Tick(AICharacterManager aiCharacter)
        {
            // If character is performing an action, dont move
            if (aiCharacter.IsPerformingAction) {
                return this;
            }

            // Check if target is null, if so throw an error
            if (aiCharacter.HasTarget == false) {
                Debug.LogError("Target Transform is null in AIPursueTargetState");
                return SwitchState(aiCharacter, aiCharacter.IdleState);
            }

            // Make sure our navmesh agent is active
            if (aiCharacter.NavMeshAgent.enabled == false) {
                aiCharacter.NavMeshAgent.enabled = true;
            }

            // If we are in attacking distance switch to attack state

            // If we have a target and we arent in attacking distance, pursue the target
            aiCharacter.NavMeshAgent.SetDestination(aiCharacter.TargetTransform.position);

            return this;
        }
    }
}
