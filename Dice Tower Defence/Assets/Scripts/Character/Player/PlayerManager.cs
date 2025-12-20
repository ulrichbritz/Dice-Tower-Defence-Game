using UnityEngine;

namespace UB
{
    public class PlayerManager : CharacterManager
    {
        [Header("Internal References")]
        [HideInInspector] public PlayerLocomotionManager PlayerLocomotionManager { get; private set; }
        [HideInInspector] public PlayerAnimatorManager PlayerAnimatorManager { get; private set; }
        [HideInInspector] public PlayerStatsManager PlayerStatsManager { get; private set; }

        [Header("External References")]
        [HideInInspector] public PlayerInputManager PlayerInputManager { get; private set; }
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            DontDestroyOnLoad(this.gameObject);

            // Internal References Initialization
            PlayerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            PlayerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            PlayerStatsManager = GetComponent<PlayerStatsManager>();

            // External References Initialization
            PlayerInputManager = PlayerInputManager.Instance;

            // Link To Reference's
            PlayerInputManager.Player = this;
        }

        protected override void Update()
        {
            base.Update();

            PlayerLocomotionManager.HandleAllMovement();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Internal References Cleanup

            // External References Cleanup and Unlink
            PlayerInputManager.Player = null;
            PlayerInputManager = null;
        }
    }
}