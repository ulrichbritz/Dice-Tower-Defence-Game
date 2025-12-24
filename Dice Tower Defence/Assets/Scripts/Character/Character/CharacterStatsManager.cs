using UnityEngine;

namespace UB
{
    public class CharacterStatsManager : MonoBehaviour
    {
        private CharacterManager characterManager;

        [SerializeField] private CharacterStats BaseCharacterStats;
        public CharacterStats CurrentCharacterStats { get; private set; }

        public bool IsDead { get; private set; }

        // Track previous health value for HUD updates
        private int previousHealth;

        protected virtual void Awake()
        {
            characterManager = GetComponent<CharacterManager>();

            // TODO this might haave to be changed when we add saving and loading
            CurrentCharacterStats = Instantiate(BaseCharacterStats);

            // Subscribe to health changes
            CurrentCharacterStats.OnCurrentHealthChanged += OnHealthChanged;
        }

        protected virtual void Start()
        {

        }

        /// <summary>
        /// Called when CurrentHealth changes. Updates the WorldSpaceHUD if available.
        /// </summary>
        /// <param name="oldHealth">The previous health value</param>
        /// <param name="newHealth">The new health value</param>
        private void OnHealthChanged(int oldHealth, int newHealth)
        {
            // Update WorldSpaceHUD if it exists
            if (characterManager.WorldSpaceHUDManager != null) {
                characterManager.WorldSpaceHUDManager.SetNewHealthValue(oldHealth, newHealth);
            }
        }

        protected virtual void OnDestroy()
        {
            // Unsubscribe from health changes
            if (CurrentCharacterStats != null) {
                CurrentCharacterStats.OnCurrentHealthChanged -= OnHealthChanged;
            }

            CurrentCharacterStats = null;
        }
    }
}
