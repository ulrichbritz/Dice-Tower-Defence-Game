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
        protected virtual void OnHealthChanged(int oldHealth, int newHealth)
        {
            if (CurrentCharacterStats.CurrentHealth > CurrentCharacterStats.MaxHealth) {
                CurrentCharacterStats.CurrentHealth = CurrentCharacterStats.MaxHealth;
            }

            // Update WorldSpaceHUD if it exists
            if (characterManager.WorldSpaceHUDManager != null) {
                characterManager.WorldSpaceHUDManager.SetNewHealthValue(oldHealth, newHealth);
            }

            if (CurrentCharacterStats.CurrentHealth <= 0 && !IsDead) {
                TriggerDeathEvent();
            }
        }

        protected virtual void TriggerDeathEvent()
        {
            CurrentCharacterStats.CurrentHealth = 0;
            IsDead = true;

            WorldRoutineManager.Instance.Run(characterManager.ProcessDeathEvent());
        }

        //protected virtual void De

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
