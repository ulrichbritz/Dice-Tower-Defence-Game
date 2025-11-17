using UnityEngine;

namespace UB
{
    [System.Serializable]
    public class DieFace
    {
        [Tooltip("The number/value on this face")]
        public int FaceNumber;
        
        [Tooltip("Rotation needed to show this face pointing up")]
        public Vector3 Rotation;
        
        [Tooltip("The normal direction of this face (used for positioning text)")]
        public Vector3 FaceNormal;
        
        [Tooltip("Local position offset for the text on this face")]
        public Vector3 TextOffset = Vector3.zero;
        
        [Tooltip("Local rotation for the text on this face")]
        public Vector3 TextRotation = Vector3.zero;
        
        // Constructor for easy creation
        public DieFace(int faceNumber, Vector3 rotation)
        {
            FaceNumber = faceNumber;
            Rotation = rotation;
            FaceNormal = Vector3.up; // Default to up
        }
        
        // Enhanced constructor with face normal
        public DieFace(int faceNumber, Vector3 rotation, Vector3 faceNormal)
        {
            FaceNumber = faceNumber;
            Rotation = rotation;
            FaceNormal = faceNormal.normalized;
        }
        
        // Default constructor
        public DieFace()
        {
            FaceNumber = 1;
            Rotation = Vector3.zero;
            FaceNormal = Vector3.up;
        }
    }
}

