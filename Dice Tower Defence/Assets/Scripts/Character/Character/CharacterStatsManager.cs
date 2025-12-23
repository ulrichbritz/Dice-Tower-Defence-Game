using UnityEngine;

namespace UB
{
    public abstract class CharacterStatsManager : MonoBehaviour
    {
        public int Health;
        public int MaxHealth;
        public int Damage;
        public float MovementSpeed;
        public float Acceleration;
        public float StoppingDistance;
    }
}
