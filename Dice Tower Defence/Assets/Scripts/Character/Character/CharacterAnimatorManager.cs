using UnityEngine;

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

        protected virtual void OnDestroy()
        {
            
        }
    }
}
