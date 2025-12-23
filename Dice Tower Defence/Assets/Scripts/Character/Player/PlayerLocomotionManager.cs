using UnityEngine;

namespace UB
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        [Header("Internal References")]
        private PlayerManager playerManager;

        [Header("Movement")]
        private float verticalMovement;
        private float horizontalMovement;
        private float moveAmount;
        private Vector3 moveDirection;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            playerManager = GetComponent<PlayerManager>();
        }

        public void HandleAllMovement()
        {
            HandleGroundedMovement();
            HandleRotation();
        }

        #region  Movement

        public void SetMovementInputs(float vertical, float horizontal, float moveAmt)
        {
            verticalMovement = vertical;
            horizontalMovement = horizontal;
            moveAmount = moveAmt;
        }

        public void HandleGroundedMovement()
        {
            // For top-down games, use world-space movement instead of camera-relative
            moveDirection = new Vector3(horizontalMovement, 0, verticalMovement);
            moveDirection = moveDirection.normalized;
            moveDirection *= moveAmount;

            // Apply movement to character controller
            if (playerManager.CharacterController != null) {
                playerManager.CharacterController.Move(moveDirection * playerManager.PlayerStatsManager.MovementSpeed * Time.deltaTime);
            }
        }

        #endregion

        #region  Rotation
        private void HandleRotation()
        {
            if (moveDirection == Vector3.zero) {
                return;
            }

            // Rotate the player to face movement direction
            Vector3 targetDirection = moveDirection;
            targetDirection.y = 0f; // Keep rotation on horizontal plane for top-down

            if (targetDirection != Vector3.zero) {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                playerManager.transform.rotation = Quaternion.Slerp(
                    playerManager.transform.rotation,
                    targetRotation,
                    Time.deltaTime * 10f); // Rotation speed
            }
        }
        #endregion

        #region Player Locomotion Actions

        public void AttemptToPerformDodge()
        {
            // TODO implement dodge
        }

        #endregion

        protected override void OnDestroy()
        {
            base.OnDestroy();

            playerManager = null;
        }
    }
}
