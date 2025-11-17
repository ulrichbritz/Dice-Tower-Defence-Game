using UnityEngine;

namespace UB
{
    public class PlayerManager : CharacterManager
    {
        [Header("Internal References")]
        [HideInInspector] public Animator Animator { get; private set; }
        [HideInInspector] public Rigidbody Rigidbody { get; private set; }

        [Header("External References")]
        [HideInInspector] public PlayerInputManager PlayerInputManager { get; private set; }
        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            base.Start();

            // Internal References Initialization
            Animator = GetComponent<Animator>();
            Rigidbody = GetComponent<Rigidbody>();

            // External References Initialization
            PlayerInputManager = PlayerInputManager.Instance;

            // Link To Reference's
            PlayerInputManager.Player = this;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            // Internal References Cleanup
            Animator = null;
            Rigidbody = null;

            // External References Cleanup and Unlink
            PlayerInputManager.Player = null;
            PlayerInputManager = null;
        }
    }
}