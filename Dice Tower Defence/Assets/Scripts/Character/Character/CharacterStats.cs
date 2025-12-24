using UnityEngine;

namespace UB
{
    public class CharacterStats : ScriptableObject
    {
        // Private backing field for CurrentHealth
        [SerializeField] private int currentHealth;

        // Property with change notification
        public int CurrentHealth
        {
            get => currentHealth;
            set  {
                if (currentHealth != value) {
                    int oldHealth = currentHealth;
                    currentHealth = value;
                    OnCurrentHealthChanged?.Invoke(oldHealth, value);
                }
            }
        }

        // Event for health changes - passes old and new values
        public System.Action<int, int> OnCurrentHealthChanged;

        public int MaxHealth;
        public float Damage;
        public float MovementSpeed;
        public float Acceleration;
        public float StoppingDistance;

        private void OnDestroy()
        {
            // Clear event subscriptions
            OnCurrentHealthChanged = null;
        }
    }
}
