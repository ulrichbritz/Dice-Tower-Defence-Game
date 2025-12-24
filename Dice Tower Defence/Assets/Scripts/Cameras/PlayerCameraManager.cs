using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.Cinemachine;
using AsyncRoutines;

namespace UB
{
    public class PlayerCameraManager : MonoBehaviour
    {
        public static PlayerCameraManager Instance { get; private set; }

        public Camera PlayerCamera;
        [Header("Cinemachine")]
        public CinemachineCamera CinemachineCamera;

        [Header("Spawn Zoom Animation Settings")]
        [SerializeField] private float normalFieldOfView = 60f; // Normal FOV for perspective
        [SerializeField] private float zoomedFieldOfView = 75f; // Zoomed out FOV for perspective
        [SerializeField] private float normalOrthoSize = 10f; // Normal size for orthographic
        [SerializeField] private float zoomedOrthoSize = 15f; // Zoomed out size for orthographic
        [SerializeField] private float zoomAnimationSpeed = 2f; // Speed of zoom animation

        [Header("Spawn Focus Animation Settings")]
        [SerializeField] private float focusFieldOfView = 65f; // Close up FOV for perspective
        [SerializeField] private float focusOrthoSize = 12f; // Close up size for orthographic
        [SerializeField] private float focusAnimationSpeed = 3f; // Speed of focus animation
        [SerializeField] private Vector3 focusOffset = new Vector3(0f, 8f, -5f); // Camera offset from target

        private Vector3 originalCameraPosition;
        private bool isAnimating = false;

        private void Awake()
        {
            if (Instance == null) {
                Instance = this;
            }
            else {
                Destroy(this.gameObject);
            }

            PlayerCamera = GetComponentInChildren<Camera>();
            CinemachineCamera = GetComponentInChildren<CinemachineCamera>();

            // Store original camera position
            originalCameraPosition = CinemachineCamera.transform.position;

            // Set the camera to normal values at start
            var lens = CinemachineCamera.Lens;
            if (PlayerCamera.orthographic) {
                lens.OrthographicSize = normalOrthoSize;
            }
            else {
                lens.FieldOfView = normalFieldOfView;
            }
            CinemachineCamera.Lens = lens;
        }

        private void Start()
        {

        }

        #region Camera Animations
        /// <summary>
        /// Zoom out briefly to show enemy spawns, then zoom back in
        /// </summary>
        /// <param name="duration">How long to stay zoomed out in seconds</param>
        public async void ZoomOutForSpawn(float duration = 3f)
        {
            if (CinemachineCamera == null || isAnimating) {
                return;
            }

            await ZoomOutRoutine(duration);
        }

        /// <summary>
        /// Focus camera on a specific spawned enemy with zoom effect
        /// </summary>
        /// <param name="enemyTransform">The enemy to focus on</param>
        /// <param name="duration">How long to focus on the enemy in seconds</param>
        public void FocusSpawn(Transform enemyTransform, float duration = 2f)
        {
            if (CinemachineCamera == null || isAnimating || enemyTransform == null) {
                return;
            }

            WorldRoutineManager.Instance.Run(FocusSpawnRoutine(enemyTransform, duration));
        }

        /// <summary>
        /// Focus camera on a specific position with zoom effect
        /// </summary>
        /// <param name="position">The position to focus on</param>
        /// <param name="duration">How long to focus on the position in seconds</param>
        public void FocusPosition(Vector3 position, float duration = 2f)
        {
            if (CinemachineCamera == null || isAnimating) {
                return;
            }

            WorldRoutineManager.Instance.Run(FocusPositionRoutine(position, duration));
        }

        /// <summary>
        /// Move camera to a specific position without returning (for spawn sequences)
        /// </summary>
        /// <param name="position">The position to move to</param>
        public void MoveToPosition(Vector3 position)
        {
            if (CinemachineCamera == null) {
                return;
            }

            WorldRoutineManager.Instance.Run(MoveToPositionRoutine(position));
        }

        /// <summary>
        /// Return camera to normal gameplay position after spawn sequence
        /// </summary>
        public void ReturnToNormalPosition()
        {
            if (CinemachineCamera == null) {
                return;
            }

            WorldRoutineManager.Instance.Run(ReturnToNormalRoutine());
        }

        private async Routine ZoomOutRoutine(float duration)
        {
            isAnimating = true;

            // Zoom out phase
            if (PlayerCamera.orthographic) {
                await AnimateOrthoSize(normalOrthoSize, zoomedOrthoSize);
            }
            else {
                await AnimateFieldOfView(normalFieldOfView, zoomedFieldOfView);
            }

            // Wait for the specified duration
            await RoutineBase.WaitForSeconds(duration);

            // Zoom back in phase
            if (PlayerCamera.orthographic) {
                await AnimateOrthoSize(zoomedOrthoSize, normalOrthoSize);
            }
            else {
                await AnimateFieldOfView(zoomedFieldOfView, normalFieldOfView);
            }

            isAnimating = false;
        }

        private async Routine AnimateFieldOfView(float fromFOV, float toFOV)
        {
            float elapsed = 0f;
            float duration = Mathf.Abs(toFOV - fromFOV) / (zoomAnimationSpeed * 10f);

            while (elapsed < duration) {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                t = Mathf.SmoothStep(0f, 1f, t); // Smooth animation curve

                var lens = CinemachineCamera.Lens;
                lens.FieldOfView = Mathf.Lerp(fromFOV, toFOV, t);
                CinemachineCamera.Lens = lens;
                await RoutineBase.WaitForNextFrame();
            }

            var finalLens = CinemachineCamera.Lens;
            finalLens.FieldOfView = toFOV;
            CinemachineCamera.Lens = finalLens;
        }

        private async Routine AnimateOrthoSize(float fromSize, float toSize)
        {
            float elapsed = 0f;
            float duration = Mathf.Abs(toSize - fromSize) / zoomAnimationSpeed;

            while (elapsed < duration) {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                t = Mathf.SmoothStep(0f, 1f, t); // Smooth animation curve

                var lens = CinemachineCamera.Lens;
                lens.OrthographicSize = Mathf.Lerp(fromSize, toSize, t);
                CinemachineCamera.Lens = lens;
                await RoutineBase.WaitForNextFrame();
            }

            var finalLens = CinemachineCamera.Lens;
            finalLens.OrthographicSize = toSize;
            CinemachineCamera.Lens = finalLens;
        }

        private async Routine FocusSpawnRoutine(Transform enemyTransform, float duration)
        {
            isAnimating = true;

            // Store current values
            Vector3 startPosition = CinemachineCamera.transform.position;
            var currentLens = CinemachineCamera.Lens;
            float startFOV = currentLens.FieldOfView;
            float startOrthoSize = currentLens.OrthographicSize;

            // Calculate target position
            Vector3 targetPosition = enemyTransform.position + focusOffset;

            // Animate to focus position and zoom
            float elapsed = 0f;
            float moveSpeed = focusAnimationSpeed;

            while (elapsed < 1f) {
                elapsed += Time.deltaTime * moveSpeed;
                float t = Mathf.SmoothStep(0f, 1f, elapsed);

                // Move camera
                CinemachineCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

                // Zoom in
                var lens = CinemachineCamera.Lens;
                if (PlayerCamera.orthographic) {
                    lens.OrthographicSize = Mathf.Lerp(startOrthoSize, focusOrthoSize, t);
                }
                else {
                    lens.FieldOfView = Mathf.Lerp(startFOV, focusFieldOfView, t);
                }
                CinemachineCamera.Lens = lens;

                await RoutineBase.WaitForNextFrame();
            }

            // Wait at focused position
            await RoutineBase.WaitForSeconds(duration);

            // Return to original position and zoom
            elapsed = 0f;
            Vector3 focusedPosition = CinemachineCamera.transform.position;
            var focusedLens = CinemachineCamera.Lens;
            float focusedFOV = focusedLens.FieldOfView;
            float focusedOrthoSize = focusedLens.OrthographicSize;

            while (elapsed < 1f) {
                elapsed += Time.deltaTime * moveSpeed;
                float t = Mathf.SmoothStep(0f, 1f, elapsed);

                // Move camera back
                CinemachineCamera.transform.position = Vector3.Lerp(focusedPosition, originalCameraPosition, t);

                // Zoom back out
                var lens = CinemachineCamera.Lens;
                if (PlayerCamera.orthographic) {
                    lens.OrthographicSize = Mathf.Lerp(focusedOrthoSize, normalOrthoSize, t);
                }
                else {
                    lens.FieldOfView = Mathf.Lerp(focusedFOV, normalFieldOfView, t);
                }
                CinemachineCamera.Lens = lens;

                await RoutineBase.WaitForNextFrame();
            }

            // Ensure final values
            CinemachineCamera.transform.position = originalCameraPosition;
            var finalLens = CinemachineCamera.Lens;
            if (PlayerCamera.orthographic) {
                finalLens.OrthographicSize = normalOrthoSize;
            }
            else {
                finalLens.FieldOfView = normalFieldOfView;
            }
            CinemachineCamera.Lens = finalLens;

            isAnimating = false;
        }

        private async Routine FocusPositionRoutine(Vector3 targetPosition, float duration)
        {
            isAnimating = true;

            // Store current values
            Vector3 startPosition = CinemachineCamera.transform.position;
            var currentLens = CinemachineCamera.Lens;
            float startFOV = currentLens.FieldOfView;
            float startOrthoSize = currentLens.OrthographicSize;

            // Calculate target position with offset
            Vector3 focusTargetPosition = targetPosition + focusOffset;

            // Animate to focus position and zoom
            float elapsed = 0f;
            float moveSpeed = focusAnimationSpeed;

            while (elapsed < 1f) {
                elapsed += Time.deltaTime * moveSpeed;
                float t = Mathf.SmoothStep(0f, 1f, elapsed);

                // Move camera
                CinemachineCamera.transform.position = Vector3.Lerp(startPosition, focusTargetPosition, t);

                // Zoom in
                var lens = CinemachineCamera.Lens;
                if (PlayerCamera.orthographic) {
                    lens.OrthographicSize = Mathf.Lerp(startOrthoSize, focusOrthoSize, t);
                }
                else {
                    lens.FieldOfView = Mathf.Lerp(startFOV, focusFieldOfView, t);
                }
                CinemachineCamera.Lens = lens;

                await RoutineBase.WaitForNextFrame();
            }

            // Wait at focused position
            await RoutineBase.WaitForSeconds(duration);

            // Return to original position and zoom
            elapsed = 0f;
            Vector3 focusedPosition = CinemachineCamera.transform.position;
            var focusedLens = CinemachineCamera.Lens;
            float focusedFOV = focusedLens.FieldOfView;
            float focusedOrthoSize = focusedLens.OrthographicSize;

            while (elapsed < 1f) {
                elapsed += Time.deltaTime * moveSpeed;
                float t = Mathf.SmoothStep(0f, 1f, elapsed);

                // Move camera back
                CinemachineCamera.transform.position = Vector3.Lerp(focusedPosition, originalCameraPosition, t);

                // Zoom back out
                var lens = CinemachineCamera.Lens;
                if (PlayerCamera.orthographic) {
                    lens.OrthographicSize = Mathf.Lerp(focusedOrthoSize, normalOrthoSize, t);
                }
                else {
                    lens.FieldOfView = Mathf.Lerp(focusedFOV, normalFieldOfView, t);
                }
                CinemachineCamera.Lens = lens;

                await RoutineBase.WaitForNextFrame();
            }

            // Ensure final values
            CinemachineCamera.transform.position = originalCameraPosition;
            var finalLens = CinemachineCamera.Lens;
            if (PlayerCamera.orthographic) {
                finalLens.OrthographicSize = normalOrthoSize;
            }
            else {
                finalLens.FieldOfView = normalFieldOfView;
            }
            CinemachineCamera.Lens = finalLens;

            isAnimating = false;
        }

        private async Routine MoveToPositionRoutine(Vector3 targetPosition)
        {
            Vector3 startPosition = CinemachineCamera.transform.position;
            Vector3 focusTargetPosition = targetPosition + focusOffset;

            var currentLens = CinemachineCamera.Lens;
            float startFOV = currentLens.FieldOfView;
            float startOrthoSize = currentLens.OrthographicSize;

            float elapsed = 0f;
            float moveSpeed = focusAnimationSpeed;

            while (elapsed < 1f) {
                elapsed += Time.deltaTime * moveSpeed;
                float t = Mathf.SmoothStep(0f, 1f, elapsed);

                // Move camera
                CinemachineCamera.transform.position = Vector3.Lerp(startPosition, focusTargetPosition, t);

                // Zoom in
                var lens = CinemachineCamera.Lens;
                if (PlayerCamera.orthographic) {
                    lens.OrthographicSize = Mathf.Lerp(startOrthoSize, focusOrthoSize, t);
                }
                else {
                    lens.FieldOfView = Mathf.Lerp(startFOV, focusFieldOfView, t);
                }
                CinemachineCamera.Lens = lens;

                await RoutineBase.WaitForNextFrame();
            }
        }

        private async Routine ReturnToNormalRoutine()
        {
            Vector3 startPosition = CinemachineCamera.transform.position;
            var currentLens = CinemachineCamera.Lens;
            float startFOV = currentLens.FieldOfView;
            float startOrthoSize = currentLens.OrthographicSize;

            float elapsed = 0f;
            float moveSpeed = focusAnimationSpeed;

            while (elapsed < 1f) {
                elapsed += Time.deltaTime * moveSpeed;
                float t = Mathf.SmoothStep(0f, 1f, elapsed);

                // Move camera back to original position
                CinemachineCamera.transform.position = Vector3.Lerp(startPosition, originalCameraPosition, t);

                // Zoom back to normal
                var lens = CinemachineCamera.Lens;
                if (PlayerCamera.orthographic) {
                    lens.OrthographicSize = Mathf.Lerp(startOrthoSize, normalOrthoSize, t);
                }
                else {
                    lens.FieldOfView = Mathf.Lerp(startFOV, normalFieldOfView, t);
                }
                CinemachineCamera.Lens = lens;

                await RoutineBase.WaitForNextFrame();
            }

            // Ensure final values
            CinemachineCamera.transform.position = originalCameraPosition;
            var finalLens = CinemachineCamera.Lens;
            if (PlayerCamera.orthographic) {
                finalLens.OrthographicSize = normalOrthoSize;
            }
            else {
                finalLens.FieldOfView = normalFieldOfView;
            }
            CinemachineCamera.Lens = finalLens;
        }
        #endregion

        private void OnDestroy()
        {
            // Clean up singleton instance
            if (Instance == this) {
                Instance = null;
            }

            // Reset animation flag
            isAnimating = false;
        }
    }
}
