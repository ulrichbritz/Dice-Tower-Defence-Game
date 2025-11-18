using AsyncRoutines;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UB
{
    /// <summary>
    /// Professional button enhancement component for UI polish
    /// Adds subtle animations and visual feedback to buttons
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class UIButtonEnhancer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        [Header("Animation Settings")]
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private float pressScale = 0.95f;
        [SerializeField] private float animationDuration = 0.15f;
        
        [Header("Visual Effects")]
        [SerializeField] private bool enableHoverGlow = true;
        [SerializeField] private Color defaultColor = new Color(0.3f, 0.3f, 0.3f, 1f); // Medium dark grey
        [SerializeField] private Color hoverColor = new Color(0.96f, 0.29f, 0.29f, 1f); // Red color (#F54B4B)
        [SerializeField] private bool enableSelectPulse = true;
        [SerializeField] private float pulseScale = 1.05f;
        [SerializeField] private float pulseSpeed = 0.15f;
        
        [Header("Audio (Optional)")]
        // TODO add audio clips
        //[SerializeField] private AudioClip hoverSound;
        //[SerializeField] private AudioClip clickSound;
        [SerializeField] private float volume = 0.5f;
        
        private Button button;
        private Image buttonImage;
        private Vector3 originalScale;
        private Color originalColor;
        private RoutineHandle pulseAnimation;
        private bool isSelected;
        private bool isHovered;
        
        private void Awake()
        {
            button = GetComponent<Button>();
            buttonImage = GetComponent<Image>();
            originalScale = transform.localScale;
            
            // Set button to default color instead of using existing color
            if (buttonImage != null) {
                originalColor = defaultColor;
                buttonImage.color = defaultColor;
            }
            
            // Disable Unity's built-in color tint to avoid conflicts
            DisableBuiltInColorTint();
            
            // Subscribe to button click for press animation
            button.onClick.AddListener(OnButtonClicked);
        }
        
        private void Start()
        {
            // Handle case where button is already selected (like New Game button)
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject) {
                isSelected = true;
                
                // Instant red color
                if (buttonImage != null) {
                    buttonImage.color = hoverColor;
                }
                
                if (enableSelectPulse) {
                    StartPulseAnimation();
                }
            }
        }
        
        private void OnDestroy()
        {
            if (button != null) {
                button.onClick.RemoveListener(OnButtonClicked);
            }
            
            StopAllAnimations();
        }
        
        #region Interface Implementations
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!button.interactable) return;
            
            // Auto-select button on hover for responsive game UI feel
            button.Select();
            
            isHovered = true;
            //PlaySound(hoverSound);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
        }
        
        public void OnSelect(BaseEventData eventData)
        {
            if (!button.interactable) return;
            
            isSelected = true;
            
            // Instant red color
            if (buttonImage != null) {
                buttonImage.color = hoverColor;
            }
            
            // Start pulse immediately
            if (enableSelectPulse) {
                StartPulseAnimation();
            }
        }
        
        public void OnDeselect(BaseEventData eventData)
        {
            isSelected = false;
            StopPulseAnimation();
            
            // Instant return to default color
            if (buttonImage != null) {
                buttonImage.color = defaultColor;
            }
        }
        
        #endregion
        
        #region Animation Methods
        

        
        private void OnButtonClicked()
        {
            if (!button.interactable) return;
            
            //PlaySound(clickSound);
        }
        

        
        private void StartPulseAnimation()
        {
            StopPulseAnimation();
            pulseAnimation = WorldRoutineManager.Instance.Run(PulseLoop());
        }
        
        private void StopPulseAnimation()
        {
            if (!pulseAnimation.IsDead) {
                pulseAnimation.Stop();
            }
        }
        
        private async Routine PulseLoop()
        {
            while (isSelected)
            {
                // Simple pulse: normal scale to slightly larger and back
                float pulseDuration = animationDuration / pulseSpeed;
                
                await UITweens.ScaleTo(transform, originalScale * pulseScale, pulseDuration);
                if (!isSelected) break; // Exit if deselected during animation
                
                await UITweens.ScaleTo(transform, originalScale, pulseDuration);
                if (!isSelected) break; // Exit if deselected during animation
            }
            
            // Ensure we're back at original scale
            transform.localScale = originalScale;
        }
        
        #endregion
        
        #region Unity Button Integration
        
        /// <summary>
        /// Disable Unity's built-in color tint system to avoid conflicts with custom animations
        /// </summary>
        private void DisableBuiltInColorTint()
        {
            if (button != null) {
                // Set transition to None to disable Unity's color management
                button.transition = Selectable.Transition.None;
            }
        }
        
        #endregion
        
        #region Utility Methods
        

        private void StopAllAnimations()
        {
            StopPulseAnimation();
        }
        
        private void PlaySound(AudioClip clip)
        {
            if (clip != null) {
                // You can replace this with your audio manager when you have one
                AudioSource.PlayClipAtPoint(clip, transform.position, volume);
            }
        }
        
        #endregion
        
        #region Public API
        
        /// <summary>
        /// Manually trigger selection (useful for custom input systems)
        /// </summary>
        public void TriggerSelect()
        {
            if (button.interactable) {
                button.Select();
            }
        }
        
        /// <summary>
        /// Reset button to default state
        /// </summary>
        public void ResetToOriginal()
        {
            StopAllAnimations();
            transform.localScale = originalScale;
            if (buttonImage != null) {
                buttonImage.color = defaultColor;
            }
        }
        
        /// <summary>
        /// Enable/disable all animations
        /// </summary>
        public void SetAnimationsEnabled(bool enabled)
        {
            this.enabled = enabled;
            if (!enabled) {
                ResetToOriginal();
            }
        }
        
        #endregion
    }
}