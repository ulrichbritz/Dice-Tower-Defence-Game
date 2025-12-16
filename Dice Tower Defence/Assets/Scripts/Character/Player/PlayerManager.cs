using UnityEngine;

namespace UB
{
    public class PlayerManager : CharacterManager
    {
        [Header("External References")]
        [HideInInspector] public PlayerInputManager PlayerInputManager { get; private set; }
        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            base.Start();

            DontDestroyOnLoad(this.gameObject);

            // Internal References Initialization

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

            // External References Cleanup and Unlink
            PlayerInputManager.Player = null;
            PlayerInputManager = null;
        }
    }
}