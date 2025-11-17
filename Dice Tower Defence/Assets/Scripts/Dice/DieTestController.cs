using UnityEditor;
using UnityEngine;

namespace UB
{
    /// <summary>
    /// Simple test script to demonstrate die functionality
    /// </summary>
    public class DieTestController : MonoBehaviour
    {
        [Header("Testing Controls")]
        [Tooltip("The die controller to test")]
        public DieController dieController;
        
        [Header("Test Options")]
        [Tooltip("Test face numbers to cycle through")]
        public int[] testFaceNumbers = {1, 2, 3, 4, 5, 6};
        
        [Tooltip("Custom numbers to test dynamic face changes")]
        public int[] customNumbers = {10, 20, 30, 40, 50, 60};
        
        private int currentTestIndex = 0;
        
        void Start()
        {
            // Auto-find die controller if not assigned
            if (dieController == null)
            {
                dieController = FindObjectOfType<DieController>();
            }
            
            if (dieController != null)
            {
                // Subscribe to events
                dieController.OnRollComplete += OnDieRollComplete;
                dieController.OnFaceNumberChanged += OnDieFaceChanged;
                
                Debug.Log("Die Test Controller initialized. Use the context menu or keyboard controls to test!");
                Debug.Log("Controls: R = Roll Random, 1-6 = Roll to specific face, C = Change to custom numbers, S = Reset to standard");
            }
            else
            {
                Debug.LogError("DieTestController: No DieController found!");
            }
        }
        
        void Update()
        {
            if (dieController == null) return;
            
            // Keyboard controls for testing
            if (Input.GetKeyDown(KeyCode.R))
            {
                TestRollRandom();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TestRollToFace(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                TestRollToFace(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                TestRollToFace(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                TestRollToFace(4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                TestRollToFace(5);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                TestRollToFace(6);
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                TestCustomNumbers();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                TestResetToStandard();
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                TestCycleFaces();
            }
        }
        
        // Event handlers
        private void OnDieRollComplete(int faceNumber)
        {
            Debug.Log($"Die roll completed! Landed on face: {faceNumber}");
        }
        
        private void OnDieFaceChanged(int oldNumber, int newNumber)
        {
            Debug.Log($"Face number changed from {oldNumber} to {newNumber}");
        }
        
        // Test methods with context menu for easy testing
        [ContextMenu("Test: Roll Random")]
        public void TestRollRandom()
        {
            if (dieController != null && !dieController.IsRolling())
            {
                Debug.Log("Testing random roll...");
                dieController.RollDie();
            }
        }
        
        [ContextMenu("Test: Roll to Face 1")]
        public void TestRollToFace1() => TestRollToFace(1);
        
        [ContextMenu("Test: Roll to Face 6")]
        public void TestRollToFace6() => TestRollToFace(6);
        
        public void TestRollToFace(int faceNumber)
        {
            if (dieController != null && !dieController.IsRolling())
            {
                Debug.Log($"Testing roll to face {faceNumber}...");
                dieController.RollToFaceNumber(faceNumber);
            }
        }
        
        [ContextMenu("Test: Set Custom Numbers")]
        public void TestCustomNumbers()
        {
            if (dieController != null)
            {
                Debug.Log($"Setting custom numbers: {string.Join(", ", customNumbers)}");
                dieController.SetAllFaceNumbers(customNumbers);
            }
        }
        
        [ContextMenu("Test: Reset to Standard")]
        public void TestResetToStandard()
        {
            if (dieController != null)
            {
                Debug.Log("Resetting to standard 1-6 die...");
                dieController.SetAllFaceNumbers(new int[] {1, 2, 3, 4, 5, 6});
            }
        }
        
        [ContextMenu("Test: Cycle Through Faces")]
        public void TestCycleFaces()
        {
            if (dieController != null && !dieController.IsRolling())
            {
                int faceToShow = testFaceNumbers[currentTestIndex % testFaceNumbers.Length];
                Debug.Log($"Cycling to face {faceToShow}...");
                dieController.RollToFaceNumber(faceToShow);
                currentTestIndex++;
            }
        }
        
        [ContextMenu("Test: Change Single Face")]
        public void TestChangeSingleFace()
        {
            if (dieController != null)
            {
                Debug.Log("Changing face 1 to 99...");
                dieController.ChangeFaceNumber(1, 99);
            }
        }
        
        [ContextMenu("Test: Get Current State")]
        public void TestGetCurrentState()
        {
            if (dieController != null)
            {
                int currentFace = dieController.GetCurrentTopFace();
                int[] allFaces = dieController.GetAllFaceNumbers();
                bool isRolling = dieController.IsRolling();
                
                Debug.Log($"Current top face: {currentFace}");
                Debug.Log($"All face numbers: {string.Join(", ", allFaces)}");
                Debug.Log($"Is rolling: {isRolling}");
            }
        }
        
        void OnGUI()
        {
            if (dieController == null) return;
            
            // Simple on-screen controls for testing
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            GUILayout.Label("Die Test Controls:", EditorGUIUtility.isProSkin ? GUI.skin.label : GUI.skin.label);
            
            if (GUILayout.Button("Roll Random (R)"))
                TestRollRandom();
            
            GUILayout.BeginHorizontal();
            for (int i = 1; i <= 6; i++)
            {
                if (GUILayout.Button(i.ToString()))
                    TestRollToFace(i);
            }
            GUILayout.EndHorizontal();
            
            if (GUILayout.Button("Custom Numbers (C)"))
                TestCustomNumbers();
            
            if (GUILayout.Button("Reset Standard (S)"))
                TestResetToStandard();
            
            if (GUILayout.Button("Cycle Faces (Space)"))
                TestCycleFaces();
            
            GUILayout.Space(10);
            GUILayout.Label($"Current Top Face: {dieController.GetCurrentTopFace()}");
            GUILayout.Label($"Is Rolling: {dieController.IsRolling()}");
            
            GUILayout.EndArea();
        }
    }
}