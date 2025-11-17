using UnityEngine;
using TMPro;

namespace UB
{
    /// <summary>
    /// Utility to create text prefabs for die faces
    /// </summary>
    public class DieTextPrefabCreator : MonoBehaviour
    {
        [Header("Prefab Settings")]
        [Tooltip("Name for the created prefab")]
        public string prefabName = "DieFaceTextPrefab";
        
        [Tooltip("Default font to use (leave empty for default)")]
        public TMP_FontAsset font;
        
        [Tooltip("Default text color")]
        public Color textColor = Color.black;
        
        [Tooltip("Default text size")]
        public float fontSize = 1f;
        
        [Tooltip("Text alignment")]
        public TextAlignmentOptions alignment = TextAlignmentOptions.Center;
        
        [ContextMenu("Create Text Prefab")]
        public void CreateTextPrefab()
        {
            // Create the GameObject
            GameObject textPrefab = new GameObject(prefabName);
            
            // Add TextMeshPro component
            TextMeshPro tmp = textPrefab.AddComponent<TextMeshPro>();
            
            // Configure the TextMeshPro
            tmp.text = "1";
            tmp.fontSize = fontSize;
            tmp.color = textColor;
            tmp.alignment = alignment;
            tmp.autoSizeTextContainer = true;
            tmp.enableAutoSizing = false; // We'll control size manually
            
            // Set font if provided
            if (font != null)
            {
                tmp.font = font;
            }
            
            // Configure the RectTransform
            RectTransform rectTransform = tmp.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(1, 1);
            
            // Reset transform
            textPrefab.transform.position = Vector3.zero;
            textPrefab.transform.rotation = Quaternion.identity;
            textPrefab.transform.localScale = Vector3.one;
            
            Debug.Log($"Text prefab '{prefabName}' created! Drag it to your Project window to save as prefab.");
            
            // Select the created object
            UnityEditor.Selection.activeGameObject = textPrefab;
        }
        
        [ContextMenu("Create and Save Prefab")]
        public void CreateAndSavePrefab()
        {
            CreateTextPrefab();
            
            // Note: Automatic prefab saving requires editor scripting
            Debug.Log("Prefab created! Now drag it from Hierarchy to Project window to save it.");
        }
    }
}