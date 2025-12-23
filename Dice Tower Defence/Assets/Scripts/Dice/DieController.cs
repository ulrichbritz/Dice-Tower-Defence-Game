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
        public Die dieData;
        
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
            
            // Initialize with face 1 on top (or first face)
            if (dieData?.DieFaces != null && dieData.DieFaces.Length > 0) {
                currentTopFace = dieData.DieFaces[0].FaceNumber;
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
        /// Animate the die rolling to the target face with bounce
        /// </summary>
        private async Routine RollAnimation(int targetFaceIndex)
        {
            isRolling = true;
            
            DieFace targetFace = dieData.DieFaces[targetFaceIndex];
            Vector3 startRotation = transform.eulerAngles;
            Vector3 targetRotation = targetFace.Rotation;
            Vector3 startPosition = transform.position;
            
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
            
            // Update current top face
            int newTopFace = targetFace.FaceNumber;
            if (newTopFace != currentTopFace) {
                int oldTopFace = currentTopFace;
                currentTopFace = newTopFace;
                OnRollComplete?.Invoke(currentTopFace);
            }
            
            isRolling = false;
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
            
            for (int i = 0; i < dieData.DieFaces.Length; i++) {
                if (dieData.DieFaces[i]?.FaceNumber == faceNumber) {
                    transform.eulerAngles = dieData.DieFaces[i].Rotation;
                    currentTopFace = faceNumber;
                    return;
                }
            }
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
    }
}