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
        private override void Awake()
        {
            base.Awake();
        }

        private override void Start()
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

        private override void Update()
        {
            base.Update();
        }

        private override void OnDestroy()
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