using System.Collections;
using AsyncRoutines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UB
{
    public class TitleScreenManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject titleScreenGroup;
        [SerializeField] private Image titleScreenBackground;
        [SerializeField] private TextMeshProUGUI titleScreenText;
        [SerializeField] private Button moveToMainMenuButton;
        [SerializeField] private GameObject mainMenuGroup;

        [Header("Title Screen Animation Settings")]
        [SerializeField] private float backgroundFadeDuration = 3.0f;
        [SerializeField] private Color targetBackgroundColor = Color.gray;
        [SerializeField] private float bounceScale = 1.2f;
        [SerializeField] private float bounceDuration = 0.3f;
        [SerializeField] private float bounceInterval = 5.0f;
        
        private RoutineHandle bounceRoutine;

        private void Awake()
        {
            // Initialize UI state - everything hidden initially
            InitializeUIState();
        }

        private void Start()
        {
            moveToMainMenuButton.onClick.AddListener(OnMoveToMainMenuClicked);
            
            // Start the opening sequence using async routine
            WorldRoutineManager.Instance.Run(OpeningSequence());
        }

        private void InitializeUIState()
        {
            // Set background to black initially
            titleScreenBackground.color = Color.black;
            
            // Hide text and button initially
            titleScreenText.gameObject.SetActive(false);
            moveToMainMenuButton.enabled = false;
            
            // Ensure main menu is hidden
            mainMenuGroup.SetActive(false);
        }

        private async Routine OpeningSequence()
        {
            // Phase 1: Fade background from black to target color
            await UITweens.FadeImageColor(titleScreenBackground, targetBackgroundColor, backgroundFadeDuration);
            
            // Phase 2: Show title text and enable button
            titleScreenText.gameObject.SetActive(true);
            moveToMainMenuButton.enabled = true;
            moveToMainMenuButton.Select();
            
            // Phase 3: Initial attention bounce
            await UITweens.BounceScale(titleScreenText.transform, bounceScale, bounceDuration);
            
            // Phase 4: Start repeating bounce loop
            bounceRoutine = WorldRoutineManager.Instance.Run(
                UITweens.RepeatingBounce(titleScreenText.transform, bounceScale, bounceDuration, bounceInterval)
            );
        }

        private void OnMoveToMainMenuClicked()
        {
            // Stop bounce animation
            if (!bounceRoutine.IsDead) {
                bounceRoutine.Stop();
            }
            
            // Hide title screen, show main menu
            titleScreenGroup.SetActive(false);
            mainMenuGroup.SetActive(true);
        }


        private void OnDestroy()
        {
            // Cleanup button listener
            if (moveToMainMenuButton != null) {
                moveToMainMenuButton.onClick.RemoveListener(OnMoveToMainMenuClicked);
            }
            
            // Stop bounce animation
            if (!bounceRoutine.IsDead) {
                bounceRoutine.Stop();
            }
        }
    }
}

