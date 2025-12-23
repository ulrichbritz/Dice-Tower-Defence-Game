using UnityEngine;

namespace UB
{
    public class CharacterStats : ScriptableObject
    {
        public int CurrentHealth;
        public int MaxHealth;
        public int Damage;
        public float MovementSpeed;
        public float Acceleration;
        public float StoppingDistance;
    }
}
