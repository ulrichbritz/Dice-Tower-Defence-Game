using UnityEngine;
using System.Collections.Generic;

namespace UB
{
    /// <summary>
    /// Handles the visual rendering of dots (pips) on each face of a die
    /// </summary>
    public class DieRenderer : MonoBehaviour
    {
        [Header("Die Configuration")]
        [Tooltip("The die data containing face information")]
        public Die dieData;
        
        [Header("Dot Settings")]
        [Tooltip("Prefab for individual dots (leave empty for auto-generated)")]
        public GameObject dotPrefab;
        
        [Tooltip("Size of each dot")]
        public float dotSize = 0.2f;
        
        [Tooltip("Spacing between dots")]
        public float dotSpacing = 0.3f;
        
        [Tooltip("Distance of dots from the die center")]
        public float dotDistance = 0.76f;
        
        [Tooltip("Color of the dots")]
        public Color dotColor = Color.black;
        
        [Header("Auto-Setup")]
        [Tooltip("Automatically create standard six-sided die face normals")]
        public bool autoSetupSixSidedDie = true;
        
        // Internal references
        private List<List<GameObject>> faceDots = new List<List<GameObject>>();
        private Dictionary<int, List<GameObject>> faceDotMap = new Dictionary<int, List<GameObject>>();
        
        // Standard cube face normals for six-sided die
        private readonly Vector3[] standardFaceNormals = new Vector3[]
        {
            Vector3.up,      // Top face
            Vector3.down,    // Bottom face
            Vector3.forward, // Front face
            Vector3.back,    // Back face
            Vector3.right,   // Right face
            Vector3.left     // Left face
        };
        
        // Standard die dot patterns (relative positions on face)
        private readonly Dictionary<int, Vector2[]> dotPatterns = new Dictionary<int, Vector2[]>()
        {
            { 1, new Vector2[] { Vector2.zero } },
            { 2, new Vector2[] { new Vector2(-0.5f, 0.5f), new Vector2(0.5f, -0.5f) } },
            { 3, new Vector2[] { new Vector2(-0.5f, 0.5f), Vector2.zero, new Vector2(0.5f, -0.5f) } },
            { 4, new Vector2[] { new Vector2(-0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-0.5f, -0.5f), new Vector2(0.5f, -0.5f) } },
            { 5, new Vector2[] { new Vector2(-0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(-0.5f, -0.5f), new Vector2(0.5f, -0.5f) } },
            { 6, new Vector2[] { new Vector2(-0.5f, 0.5f), new Vector2(-0.5f, 0f), new Vector2(-0.5f, -0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0f), new Vector2(0.5f, -0.5f) } }
        };
        
        void Start()
        {
            SetupDieFaces();
        }
        
        /// <summary>
        /// Initialize the die faces with display objects (dots or text)
        /// </summary>
        public void SetupDieFaces()
        {
            if (dieData == null) {
                Debug.LogError("DieRenderer: No die data assigned!");
                return;
            }
            
            // Clear existing display objects
            ClearFaceTexts();
            
            // Auto-setup face normals for six-sided die if enabled
            if (autoSetupSixSidedDie && dieData.DieFaces != null && dieData.DieFaces.Length == 6) {
                SetupStandardSixSidedDie();
            }
            
            // Create display objects for each face
            CreateFaceTexts();
        }
        
        /// <summary>
        /// Setup standard six-sided die with proper face normals
        /// </summary>
        private void SetupStandardSixSidedDie()
        {
            for (int i = 0; i < dieData.DieFaces.Length && i < standardFaceNormals.Length; i++) {
                if (dieData.DieFaces[i] != null) {
                    dieData.DieFaces[i].FaceNormal = standardFaceNormals[i];
                }
            }
        }
        
        /// <summary>
        /// Create dot objects for each die face
        /// </summary>
        private void CreateFaceTexts()
        {
            if (dieData.DieFaces == null) return;
            
            for (int i = 0; i < dieData.DieFaces.Length; i++) {
                DieFace face = dieData.DieFaces[i];
                if (face == null) continue;
                
                List<GameObject> dots = CreateFaceDots(face, i);
                if (dots != null && dots.Count > 0) {
                    faceDots.Add(dots);
                    faceDotMap[face.FaceNumber] = dots;
                }
            }
        }
        
        /// <summary>
        /// Create dot objects for a specific face with proper orientation
        /// </summary>
        private List<GameObject> CreateFaceDots(DieFace face, int faceIndex)
        {
            List<GameObject> dots = new List<GameObject>();
            
            // Get the dot pattern for this face number
            if (!dotPatterns.ContainsKey(face.FaceNumber)) {
                return dots;
            }
            
            Vector2[] pattern = dotPatterns[face.FaceNumber];
            
            // Create individual dots positioned and oriented for this face
            for (int i = 0; i < pattern.Length; i++) {
                GameObject dot = CreateSingleDot(pattern[i], face);
                dots.Add(dot);
            }
            
            return dots;
        }
        
        /// <summary>
        /// Create a single dot positioned flat against the specified face
        /// </summary>
        private GameObject CreateSingleDot(Vector2 localPosition, DieFace face)
        {
            GameObject dot;
            
            // Use dot prefab if available, otherwise create a flat cylinder
            if (dotPrefab != null) {
                dot = Instantiate(dotPrefab, transform);
            }
            else {
                // Create a flat cylinder for round dots
                dot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                dot.transform.SetParent(transform);
                
                // Remove collider as we don't need it for visual dots
                Collider dotCollider = dot.GetComponent<Collider>();
                if (dotCollider != null) {
                    Destroy(dotCollider);
                }
            }
            
            // Calculate the dot's position on the face
            Vector3 faceCenter = face.FaceNormal * dotDistance;
            
            // Create local coordinate system for the face
            Vector3 rightVector, upVector;
            GetFaceCoordinateSystem(face.FaceNormal, out rightVector, out upVector);
            
            // Position the dot on the face using the local coordinate system
            Vector3 dotPosition = faceCenter + 
                                  (rightVector * localPosition.x * dotSpacing) + 
                                  (upVector * localPosition.y * dotSpacing);
            
            dot.transform.localPosition = dotPosition;
            
            // Orient the dot to be flat against the face
            if (dotPrefab == null) {
                // For cylinders, we need to align the Y-axis (height) with the face normal
                // Use FromToRotation to rotate from Vector3.up to face normal
                dot.transform.rotation = Quaternion.FromToRotation(Vector3.up, face.FaceNormal);
                
                // Make it flat and properly sized
                dot.transform.localScale = new Vector3(dotSize, dotSize * 0.05f, dotSize);
            }
            else {
                // For custom prefabs, orient toward the face normal
                dot.transform.rotation = Quaternion.LookRotation(face.FaceNormal, upVector);
                dot.transform.localScale = Vector3.one * dotSize;
            }
            
            // Set color to match text color
            Renderer dotRenderer = dot.GetComponent<Renderer>();
            if (dotRenderer != null) {
                dotRenderer.material.color = dotColor;
            }
            
            dot.name = $"Face_{face.FaceNumber}_Dot_{localPosition.x}_{localPosition.y}";
            
            return dot;
        }
        
        /// <summary>
        /// Get the local coordinate system for a face (right and up vectors)
        /// </summary>
        private void GetFaceCoordinateSystem(Vector3 faceNormal, out Vector3 rightVector, out Vector3 upVector)
        {
            // Define consistent coordinate systems for each face
            if (faceNormal == Vector3.up) {
                rightVector = Vector3.right;
                upVector = Vector3.forward;
            }
            else if (faceNormal == Vector3.down) {
                rightVector = Vector3.right;
                upVector = Vector3.back;
            }
            else if (faceNormal == Vector3.forward) {
                rightVector = Vector3.right;
                upVector = Vector3.up;
            }
            else if (faceNormal == Vector3.back) {
                rightVector = Vector3.left;
                upVector = Vector3.up;
            }
            else if (faceNormal == Vector3.right) {
                rightVector = Vector3.back;
                upVector = Vector3.up;
            }
            else if (faceNormal == Vector3.left) {
                rightVector = Vector3.forward;
                upVector = Vector3.up;
            }
            else {
                // Fallback for arbitrary normals
                rightVector = Vector3.Cross(Vector3.up, faceNormal).normalized;
                if (rightVector.magnitude < 0.1f)
                    rightVector = Vector3.Cross(Vector3.forward, faceNormal).normalized;
                upVector = Vector3.Cross(faceNormal, rightVector).normalized;
            }
        }
        
        /// <summary>
        /// Position text on the correct face of the die
        /// </summary>
        
        /// <summary>
        /// Clear all existing dot objects
        /// </summary>
        private void ClearFaceTexts()
        {
            // Clear dot objects
            foreach (List<GameObject> dotList in faceDots) {
                if (dotList != null) {
                    foreach (GameObject dot in dotList) {
                        if (dot != null) {
                            if (Application.isPlaying) {
                                Destroy(dot);
                            } 
                        }
                    }
                }
            }
            
            faceDots.Clear();
            faceDotMap.Clear();
        }
        
        /// <summary>
        /// Change the dot color of all faces
        /// </summary>
        public void SetDotColor(Color color)
        {
            dotColor = color;
            foreach (List<GameObject> dotList in faceDots) {
                if (dotList != null) {
                    foreach (GameObject dot in dotList) {
                        if (dot != null) {
                            Renderer renderer = dot.GetComponent<Renderer>();
                            if (renderer != null) {
                                renderer.material.color = color;
                            }
                        }
                    }
                }
            }
        }
        
        // Editor validation
        void OnValidate()
        {
            // Only run in play mode and not on prefab assets
            if (Application.isPlaying && dieData != null && gameObject.scene.isLoaded)
            {
                ClearFaceTexts();
                CreateFaceTexts();
            }
        }
        
        // Context menu helpers for testing
        [ContextMenu("Refresh All Faces")]
        public void ContextRefreshFaces()
        {
            ClearFaceTexts();
            CreateFaceTexts();
        }
    }
}