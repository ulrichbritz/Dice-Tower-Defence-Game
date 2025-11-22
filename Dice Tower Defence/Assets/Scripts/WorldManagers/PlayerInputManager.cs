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

        //[Header("Player Actions")]

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
                // Camera
                //playerControls.PlayerCamera.Movement.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();

                // Player Actions
                //playerControls.PlayerActions.Roll.performed += ctx => dodgeInput = true;
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
            // Player Actions
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
