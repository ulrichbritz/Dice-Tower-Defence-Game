using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

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

        }

        protected virtual void Start()
        {
            // Internal References Initialization
            // Components
            Animator = GetComponent<Animator>();
            Rigidbody = GetComponent<Rigidbody>();
            CharacterController = GetComponent<CharacterController>();
            AudioSource = GetComponent<AudioSource>();

            // Scripts
            CharacterStatsManager = GetComponent<CharacterStatsManager>();

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
