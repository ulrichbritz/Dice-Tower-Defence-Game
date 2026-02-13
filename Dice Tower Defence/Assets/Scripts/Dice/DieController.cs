using UnityEngine;
using System.Collections;
using AsyncRoutines;

namespace UB
{
    /// <summary>
    /// Controls die behavior, rolling animations, and dynamic face number changes
    /// </summary>
    [RequireComponent(typeof(DieRenderer))]
    public class DieController : MonoBehaviour
    {
        [Header("Die Configuration")]
        [Tooltip("The die data this controller manages")]
        public DieItem dieData;

        [Header("Animation Settings")]
        [Tooltip("Duration of roll animation in seconds")]
        public float rollDuration = 0.2f;

        [Tooltip("Number of spins during roll animation")]
        public int rollSpins = 5;

        [Tooltip("Height of the bounce during roll")]
        public float bounceHeight = 0.5f;

        [Tooltip("Number of bounces")]
        public int bounceCount = 2;

        [Tooltip("Easing curve for roll animation")]
        public AnimationCurve rollCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Tooltip("Bounce curve for vertical movement")]
        public AnimationCurve bounceCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Tooltip("Randomize rotation axes for more natural rolling")]
        public bool randomizeRotation = true;

        [Header("Face Management")]
        [Tooltip("Allow runtime changes to face numbers")]
        public bool allowFaceNumberChanges = true;

        [Header("Debug & Calibration")]
        [SerializeField] private bool showDebugInfo = false;
        [SerializeField] private bool enableManualTesting = false;

        // Components
        private DieRenderer dieRenderer;
        private Rigidbody dieRigidbody;

        // State
        private bool isRolling = false;
        private int currentTopFace = 1;
        private Vector3 originalPosition;

        // Events
        public System.Action<int> OnRollComplete;
        public System.Action<int, int> OnFaceNumberChanged; // oldNumber, newNumber

        void Awake()
        {
            dieRenderer = GetComponent<DieRenderer>();
            dieRigidbody = GetComponent<Rigidbody>();

            // Ensure die renderer has the same die data
            if (dieRenderer != null && dieData != null) {
                dieRenderer.dieData = dieData;
            }
        }

        void Start()
        {
            // Store original position for bounce animation
            originalPosition = transform.position;

            // Initialize with face 1 on top using corrected rotation
            if (dieData?.DieFaces != null && dieData.DieFaces.Length > 0) {
                currentTopFace = 1; // Default to face 1
                SetFaceInstant(currentTopFace); // Apply correct rotation immediately
            }
        }

        /// <summary>
        /// Roll the die to show a random face
        /// </summary>
        public void RollDie()
        {
            if (isRolling || dieData?.DieFaces == null || dieData.DieFaces.Length == 0)
                return;

            int randomFaceIndex = Random.Range(0, dieData.DieFaces.Length);
            RollToFace(randomFaceIndex);
        }

        /// <summary>
        /// Roll the die and return the result after animation completes
        /// </summary>
        /// <returns>The face number that was rolled</returns>
        public async Routine<int> RollDieAndGetResult()
        {
            if (isRolling || dieData?.DieFaces == null || dieData.DieFaces.Length == 0)
                return 0;

            int randomFaceIndex = Random.Range(0, dieData.DieFaces.Length);
            return await RollToFaceAndGetResult(randomFaceIndex);
        }

        /// <summary>
        /// Roll the die to a specific face number and return the result
        /// </summary>
        /// <param name="faceNumber">The face number to roll to</param>
        /// <returns>The face number that was rolled</returns>
        public async Routine<int> RollToFaceNumberAndGetResult(int faceNumber)
        {
            if (isRolling || dieData?.DieFaces == null)
                return 0;

            // Find the face index for the given number
            for (int i = 0; i < dieData.DieFaces.Length; i++) {
                if (dieData.DieFaces[i]?.FaceNumber == faceNumber) {
                    return await RollToFaceAndGetResult(i);
                }
            }

            Debug.LogWarning($"DieController: Face number {faceNumber} not found on die!");
            return 0;
        }

        /// <summary>
        /// Roll the die to show a specific face number
        /// </summary>
        public void RollToFaceNumber(int faceNumber)
        {
            if (isRolling || dieData?.DieFaces == null)
                return;

            // Find the face index for the given number
            for (int i = 0; i < dieData.DieFaces.Length; i++) {
                if (dieData.DieFaces[i]?.FaceNumber == faceNumber) {
                    RollToFace(i);
                    return;
                }
            }

            Debug.LogWarning($"DieController: Face number {faceNumber} not found on die!");
        }

        /// <summary>
        /// Roll the die to show a specific face by index
        /// </summary>
        public void RollToFace(int faceIndex)
        {
            if (isRolling || dieData?.DieFaces == null || faceIndex < 0 || faceIndex >= dieData.DieFaces.Length)
                return;

            WorldRoutineManager.Instance.Run(RollAnimation(faceIndex));
        }

        /// <summary>
        /// Roll the die to show a specific face by index and return the result
        /// </summary>
        /// <param name="faceIndex">The index of the face to roll to</param>
        /// <returns>The face number that was rolled</returns>
        public async Routine<int> RollToFaceAndGetResult(int faceIndex)
        {
            if (isRolling || dieData?.DieFaces == null || faceIndex < 0 || faceIndex >= dieData.DieFaces.Length)
                return 0;

            await RollAnimation(faceIndex);
            return currentTopFace;
        }

        /// <summary>
        /// Animate the die rolling to the target face with bounce
        /// </summary>
        private async Routine RollAnimation(int targetFaceIndex)
        {
            isRolling = true;

            DieFace targetFace = dieData.DieFaces[targetFaceIndex];
            Vector3 startRotation = transform.eulerAngles;
            Vector3 targetRotation = GetCorrectedRotationForFace(targetFace.FaceNumber);
            Vector3 startPosition = transform.position;

            Debug.Log($"Rolling to face {targetFace.FaceNumber} with corrected rotation {targetRotation}");

            // Generate spin rotation (more controlled for bounce effect)
            Vector3 spinRotation;
            if (randomizeRotation) {
                spinRotation = new Vector3(
                    Random.Range(-180, 180) * rollSpins,
                    Random.Range(-180, 180) * rollSpins,
                    Random.Range(-180, 180) * rollSpins
                );
            }
            else {
                spinRotation = new Vector3(360 * rollSpins, 360 * rollSpins, 0);
            }

            Vector3 finalRotation = targetRotation + spinRotation;

            float elapsed = 0f;

            while (elapsed < rollDuration) {
                elapsed += Time.deltaTime;
                float progress = elapsed / rollDuration;

                // Rotation animation
                float rotationProgress = rollCurve.Evaluate(progress);
                Vector3 currentRotation = Vector3.Lerp(startRotation, finalRotation, rotationProgress);
                transform.eulerAngles = currentRotation;

                // Bounce animation
                float bounceProgress = bounceCurve.Evaluate(progress);
                float bounceOffset = CalculateBounce(progress) * bounceHeight;
                Vector3 currentPosition = startPosition + Vector3.up * bounceOffset;
                transform.position = currentPosition;

                await RoutineBase.WaitForNextFrame();
            }

            // Ensure exact final rotation and position
            transform.eulerAngles = targetRotation;
            transform.position = originalPosition;

            Debug.Log($"Final rotation set to: {targetRotation}, should show face {targetFace.FaceNumber}");

            // Update current top face
            int newTopFace = targetFace.FaceNumber;
            if (newTopFace != currentTopFace) {
                int oldTopFace = currentTopFace;
                currentTopFace = newTopFace;
                OnRollComplete?.Invoke(currentTopFace);
            }
            else {
                // Still fire the event even if it's the same face
                OnRollComplete?.Invoke(currentTopFace);
            }

            isRolling = false;
        }

        /// <summary>
        /// Get the correct rotation for a face number, using standard cube orientations
        /// <summary>
        /// Get the correct rotation for a face number, using standard cube orientations
        /// </summary>
        private Vector3 GetCorrectedRotationForFace(int faceNumber)
        {
            // Corrected cube rotations based on your specific die model
            switch (faceNumber) {
                case 1: return new Vector3(0, 0, 0);       // Top
                case 2: return new Vector3(90, 0, 0);      // Front
                case 3: return new Vector3(-90, 0, 0);     // Right
                case 4: return new Vector3(0, 0, 90);      // Left
                case 5: return new Vector3(180, 0, 0);     // Back (swapped with 6)
                case 6: return new Vector3(0, 0, -90);     // Bottom (swapped with 5)
                default: return Vector3.zero;
            }
        }

        /// <summary>
        /// Calculate bounce height based on progress
        /// </summary>
        private float CalculateBounce(float progress)
        {
            // Create multiple bounces that decrease in height
            float bouncePhase = progress * bounceCount * Mathf.PI;
            float heightMultiplier = 1f - progress; // Bounces get smaller over time
            return Mathf.Sin(bouncePhase) * heightMultiplier;
        }

        /// <summary>
        /// Instantly set the die to show a specific face (no animation)
        /// </summary>
        public void SetFaceInstant(int faceNumber)
        {
            if (dieData?.DieFaces == null)
                return;

            // Use corrected rotation instead of DieFace.Rotation
            Vector3 correctedRotation = GetCorrectedRotationForFace(faceNumber);
            transform.eulerAngles = correctedRotation;
            currentTopFace = faceNumber;

            Debug.Log($"Set die instantly to face {faceNumber} with rotation {correctedRotation}");
        }

        /// <summary>
        /// Change the number on a specific face dynamically
        /// </summary>
        public void ChangeFaceNumber(int oldNumber, int newNumber)
        {
            if (!allowFaceNumberChanges || dieData?.DieFaces == null)
                return;

            // Find and update the face
            for (int i = 0; i < dieData.DieFaces.Length; i++) {
                if (dieData.DieFaces[i]?.FaceNumber == oldNumber) {
                    dieData.DieFaces[i].FaceNumber = newNumber;

                    // Update the renderer by recreating dots
                    if (dieRenderer != null) {
                        dieRenderer.ContextRefreshFaces();
                    }

                    // Update current top face if it was changed
                    if (currentTopFace == oldNumber) {
                        currentTopFace = newNumber;
                    }

                    OnFaceNumberChanged?.Invoke(oldNumber, newNumber);
                    return;
                }
            }
        }

        /// <summary>
        /// Set all face numbers at once
        /// </summary>
        public void SetAllFaceNumbers(int[] numbers)
        {
            if (!allowFaceNumberChanges || dieData?.DieFaces == null || numbers == null)
                return;

            int count = Mathf.Min(numbers.Length, dieData.DieFaces.Length);

            for (int i = 0; i < count; i++) {
                if (dieData.DieFaces[i] != null) {
                    int oldNumber = dieData.DieFaces[i].FaceNumber;
                    dieData.DieFaces[i].FaceNumber = numbers[i];

                    if (currentTopFace == oldNumber) {
                        currentTopFace = numbers[i];
                    }
                }
            }

            // Refresh all face displays
            if (dieRenderer != null) {
                dieRenderer.ContextRefreshFaces();
            }
        }

        /// <summary>
        /// Get the current top face number
        /// </summary>
        public int GetCurrentTopFace()
        {
            return currentTopFace;
        }

        /// <summary>
        /// Check if the die is currently rolling
        /// </summary>
        public bool IsRolling()
        {
            return isRolling;
        }

        /// <summary>
        /// Get all current face numbers
        /// </summary>
        public int[] GetAllFaceNumbers()
        {
            if (dieData?.DieFaces == null)
                return new int[0];

            int[] numbers = new int[dieData.DieFaces.Length];
            for (int i = 0; i < dieData.DieFaces.Length; i++) {
                numbers[i] = dieData.DieFaces[i]?.FaceNumber ?? 0;
            }

            return numbers;
        }

        /// <summary>
        /// Debug method to test all face rotations
        /// </summary>
        [ContextMenu("Test All Face Rotations")]
        public void TestAllFaceRotations()
        {
            if (dieData?.DieFaces == null) return;

            Debug.Log($"Testing {dieData.DieFaces.Length} face rotations:");
            for (int i = 0; i < dieData.DieFaces.Length; i++) {
                DieFace face = dieData.DieFaces[i];
                Debug.Log($"Face {face.FaceNumber}: Rotation {face.Rotation}");
            }
        }

        /// <summary>
        /// Manually set die to specific face for testing (Editor only)
        /// </summary>
        public void TestShowFace(int faceNumber)
        {
            if (!enableManualTesting) return;

            SetFaceInstant(faceNumber);
            Debug.Log($"Die set to show face: {faceNumber} (Current top face: {currentTopFace})");
        }

        /// <summary>
        /// Check what face is currently on top based on the transform rotation
        /// </summary>
        public int DetectCurrentTopFace()
        {
            if (dieData?.DieFaces == null) return 0;

            // Find which face rotation is closest to current rotation
            float closestAngle = float.MaxValue;
            int closestFace = 1;

            Vector3 currentEuler = transform.eulerAngles;

            foreach (DieFace face in dieData.DieFaces) {
                Vector3 faceEuler = face.Rotation;

                // Calculate angular difference
                float angleDiff = Quaternion.Angle(
                    Quaternion.Euler(currentEuler),
                    Quaternion.Euler(faceEuler)
                );

                if (angleDiff < closestAngle) {
                    closestAngle = angleDiff;
                    closestFace = face.FaceNumber;
                }
            }

            return closestFace;
        }

        void OnGUI()
        {
            if (!showDebugInfo) return;

            GUILayout.BeginArea(new Rect(Screen.width - 250, 10, 240, 300));
            GUILayout.Label($"Die Debug Info");
            GUILayout.Label($"Current Top Face: {currentTopFace}");
            GUILayout.Label($"Detected Face: {DetectCurrentTopFace()}");
            GUILayout.Label($"Is Rolling: {isRolling}");
            GUILayout.Label($"Transform Rotation: {transform.eulerAngles}");

            if (enableManualTesting && !isRolling) {
                GUILayout.Label("Manual Testing:");
                if (GUILayout.Button("Show Face 1")) TestShowFace(1);
                if (GUILayout.Button("Show Face 2")) TestShowFace(2);
                if (GUILayout.Button("Show Face 3")) TestShowFace(3);
                if (GUILayout.Button("Show Face 4")) TestShowFace(4);
                if (GUILayout.Button("Show Face 5")) TestShowFace(5);
                if (GUILayout.Button("Show Face 6")) TestShowFace(6);
                if (GUILayout.Button("Random Roll")) RollDie();
            }

            GUILayout.EndArea();
        }

        /// <summary>
        /// Example usage of the new async methods
        /// </summary>
        [System.ObsoleteAttribute("This is just an example method showing usage. Remove in production.")]
        public async void ExampleUsage()
        {
            // Roll and wait for result
            int result = await RollDieAndGetResult();
            Debug.Log($"Rolled: {result}");

            // Roll specific number and wait for result
            int sixResult = await RollToFaceNumberAndGetResult(6);
            Debug.Log($"Rolled for 6, got: {sixResult}");

            // You can now use these results immediately for damage calculations, etc.
        }
    }
}
