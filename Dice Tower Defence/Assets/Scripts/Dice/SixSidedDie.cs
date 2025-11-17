using UnityEngine;

namespace UB
{
    [CreateAssetMenu(fileName = "New Six Sided Die", menuName = "Dice/Six-Sided/Die")]
    public class SixSidedDie : Die
    {
        private void OnEnable()
        {
            // Auto-initialize with standard six-sided die setup if not already configured
            if (DieFaces == null || DieFaces.Length == 0)
            {
                InitializeStandardSixSidedDie();
            }
        }
        
        private void OnValidate()
        {
            // Ensure we always have exactly 6 face values
            if (DieFaces == null || DieFaces.Length != 6) {
                Debug.LogWarning("SixSidedDie: Must have exactly 6 face values! Auto-initializing...");
                InitializeStandardSixSidedDie();
            }
        }
        
        /// <summary>
        /// Initialize this die with standard six-sided die configuration
        /// </summary>
        private void InitializeStandardSixSidedDie()
        {
            DieName = "Six-Sided Die";
            Description = "A standard six-sided die with faces numbered 1-6";
            
            // Create the six faces with proper rotations and normals
            DieFaces = new DieFace[6];
            
            // Standard cube face configuration
            // Face 1 - Top (no rotation needed)
            DieFaces[0] = new DieFace(1, Vector3.zero, Vector3.up);
            
            // Face 6 - Bottom (180° rotation around X axis)
            DieFaces[1] = new DieFace(6, new Vector3(180, 0, 0), Vector3.down);
            
            // Face 2 - Front (90° rotation around X axis)
            DieFaces[2] = new DieFace(2, new Vector3(90, 0, 0), Vector3.forward);
            
            // Face 5 - Back (-90° rotation around X axis)
            DieFaces[3] = new DieFace(5, new Vector3(-90, 0, 0), Vector3.back);
            
            // Face 3 - Right (-90° rotation around Z axis)
            DieFaces[4] = new DieFace(3, new Vector3(0, 0, -90), Vector3.right);
            
            // Face 4 - Left (90° rotation around Z axis)
            DieFaces[5] = new DieFace(4, new Vector3(0, 0, 90), Vector3.left);
        }
        
        /// <summary>
        /// Validate that this die has exactly 6 faces
        /// </summary>
        public bool IsValid()
        {
            return DieFaces != null && DieFaces.Length == 6;
        }
        
        /// <summary>
        /// Reset to standard six-sided die configuration
        /// </summary>
        [ContextMenu("Reset to Standard Configuration")]
        public void ResetToStandard()
        {
            InitializeStandardSixSidedDie();
        }
        
        /// <summary>
        /// Set custom face numbers while keeping standard rotations
        /// </summary>
        public void SetCustomFaceNumbers(int[] faceNumbers)
        {
            if (faceNumbers == null || faceNumbers.Length != 6)
            {
                Debug.LogError("SixSidedDie: Must provide exactly 6 face numbers!");
                return;
            }
            
            // Ensure we have the standard configuration first
            if (DieFaces == null || DieFaces.Length != 6)
            {
                InitializeStandardSixSidedDie();
            }
            
            // Update face numbers while keeping rotations and normals
            for (int i = 0; i < 6; i++)
            {
                DieFaces[i].FaceNumber = faceNumbers[i];
            }
        }
    }
}

