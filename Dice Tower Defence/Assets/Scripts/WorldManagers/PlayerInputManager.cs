using UnityEngine;
using UnityEngine.SceneManagement;

namespace UB
{
    public class PlayerInputManager : WorldManager<PlayerInputManager>
    {
        [Header("Internal References")]
        private PlayerControls playerControls;

        [Header("External References")]
        [HideInInspector] public PlayerManager Player { get; set; }

        [Header("PlayerMovement")]
        private Vector2 movementInput;
        private float verticalInput;
        private float horizontalInput;
        private float moveAmount;

        [Header("Player Actions")]
        private bool dodgeInput;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            SceneManager.activeSceneChanged += OnSceneChange;

            Instance.enabled = false;

            if (playerControls != null) {
                playerControls.Disable();
            }
        }

        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            // Enable playercontrols in world scene only
            if (newScene.buildIndex == WorldSceneManager.Instance.WorldSceneIndex) {
                Instance.enabled = true;

                if (playerControls != null) {
                    playerControls.Enable();
                }
            }
            else {
                Instance.enabled = false;

                if (playerControls != null) {
                    playerControls.Disable();
                }
            }
        }

        private void OnEnable()
        {
            if (playerControls == null) {
                playerControls = new PlayerControls();
                // Movement
                playerControls.PlayerMovement.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();

                // Player Actions
                playerControls.PlayerActions.Dodge.performed += ctx => dodgeInput = true;
            }

            playerControls.Enable();
        }

        protected override void Update()
        {
            base.Update();

            HandleAllInputs();
        }

        private void HandleAllInputs()
        {
            if (Player == null) {
                return;
            }

            // Movement Input
            HandleMovementInput();
            // Player Actions
            HandleDodgeInput();
        }

        private void HandleMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;

            // Return absolute numbers to ensure positive values
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

            if (moveAmount <= 0.5f && moveAmount > 0f) {
                moveAmount = 0.5f;
            }
            else if (moveAmount > 0.5f && moveAmount <= 1f) {
                moveAmount = 1f;
            }

            if (moveAmount != 0) {
                Player.IsMoving = true;
            }
            else {
                Player.IsMoving = false;
            }

            Player.PlayerLocomotionManager.SetMovementInputs(verticalInput, horizontalInput, moveAmount);
            Player.PlayerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount);
        }

        private void HandleDodgeInput()
        {
            if (dodgeInput) {
                dodgeInput = false;
                Player.PlayerLocomotionManager.AttemptToPerformDodge();
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (enabled) {
                if (focus) {
                    playerControls.Enable();
                }
                else {
                    playerControls.Disable();
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SceneManager.activeSceneChanged -= OnSceneChange;

            Player = null;
            playerControls = null;
        }
    }
}
