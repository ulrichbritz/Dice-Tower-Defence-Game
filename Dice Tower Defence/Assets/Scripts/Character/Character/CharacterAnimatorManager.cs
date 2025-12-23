using UnityEngine;
using UnityEngine.TextCore.Text;

namespace UB
{
    public abstract class CharacterAnimatorManager : MonoBehaviour
    {
        private CharacterManager characterManager;

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {
            characterManager = GetComponent<CharacterManager>();
        }

        public virtual void UpdateAnimatorMovementParameters(float horitontalValue, float verticalValue)
        {
            characterManager.Animator.SetFloat("Horizontal", horitontalValue, 0.1f, Time.deltaTime);
            characterManager.Animator.SetFloat("Vertical", verticalValue, 0.1f, Time.deltaTime);
        }

        public virtual void PlayTargetActionAnimation(string targetAnimation, bool isPerformingAction, bool applyRootMotion, bool canMove, bool canRotate)
        {
            characterManager.IsPerformingAction = isPerformingAction;
            characterManager.CanMove = canMove;
            characterManager.CanRotate = canRotate;
            characterManager.Animator.applyRootMotion = applyRootMotion;
            characterManager.Animator.CrossFade(targetAnimation, 0.2f);
        }

        protected virtual void OnDestroy()
        {

        }
    }
}
