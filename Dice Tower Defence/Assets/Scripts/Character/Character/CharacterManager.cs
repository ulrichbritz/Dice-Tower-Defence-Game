using UnityEngine;
using UnityEngine.AI;

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