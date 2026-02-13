using AsyncRoutines;
using UB.UI;
using UnityEngine;

namespace UB
{
    public abstract class CharacterManager : MonoBehaviour
    {
        [Header("Internal References")]
        // Components
        [HideInInspector] public Animator Animator { get; private set; }
        [HideInInspector] public Rigidbody Rigidbody { get; private set; }
        [HideInInspector] public CharacterController CharacterController { get; private set; }
        [HideInInspector] public AudioSource AudioSource { get; private set; }

        //Scripts
        [HideInInspector] public CharacterStatsManager CharacterStatsManager { get; private set; }
        [HideInInspector] public CharacterEffectsManager CharacterEffectsManager { get; private set; }
        [HideInInspector] public CharacterAnimatorManager CharacterAnimatorManager { get; private set; }
        [HideInInspector] public CharacterInventoryManager CharacterInventoryManager { get; private set; }
        [HideInInspector] public WorldSpaceHUDManager WorldSpaceHUDManager { get; private set; }

        [Header("Flags")]
        [HideInInspector] public bool IsPerformingAction;
        [HideInInspector] public bool CanMove = true;
        [HideInInspector] public bool CanRotate = true;

        // IsMoving with value change callback
        private bool _isMoving;
        public bool IsMoving
        {
            get => _isMoving;
            set {
                if (_isMoving != value) {
                    _isMoving = value;
                    OnIsMovingChanged(value);
                }
            }
        }

        protected virtual void Awake()
        {
            // Internal References Initialization
            // Components
            Animator = GetComponent<Animator>();
            Rigidbody = GetComponent<Rigidbody>();
            CharacterController = GetComponent<CharacterController>();
            AudioSource = GetComponent<AudioSource>();

            // Scripts
            CharacterStatsManager = GetComponent<CharacterStatsManager>();
            CharacterEffectsManager = GetComponent<CharacterEffectsManager>();
            CharacterAnimatorManager = GetComponent<CharacterAnimatorManager>();
            CharacterInventoryManager = GetComponent<CharacterInventoryManager>();
            WorldSpaceHUDManager = GetComponentInChildren<WorldSpaceHUDManager>();
        }

        protected virtual void Start()
        {
            // Initialize IsMoving
            _isMoving = false;
        }

        protected virtual void Update()
        {

        }

        protected virtual void FixedUpdate()
        {

        }

        protected virtual void LateUpdate()
        {

        }

        public virtual async Routine ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            // todo reset flags that need to be reset

            // todo depending on weapon type, play different death animation
            if (!manuallySelectDeathAnimation) {
                CharacterAnimatorManager.PlayTargetActionAnimation("Death_F_1H", true, true, false, false);
            }

            // todo play death sfx

            await RoutineBase.WaitForSeconds(5f);

            // todo check for potential repawn modifiers or something

            // todo maybe xp or something, or update stats

            // todo Disable Character Model
        }

        /// <summary>
        /// Called when IsMoving changes. Override in derived classes to handle movement state changes.
        /// </summary>
        /// <param name="isMoving">True if character started moving, false if stopped</param>
        protected virtual void OnIsMovingChanged(bool isMoving)
        {
            Animator.SetBool("IsMoving", isMoving);
        }

        protected virtual void OnDestroy()
        {
            // Internal References Cleanup
            Animator = null;
            Rigidbody = null;
            CharacterController = null;
            AudioSource = null;
            CharacterStatsManager = null;
        }
    }
}
