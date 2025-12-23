using UnityEngine;

namespace UB
{
    public abstract class CharacterStatsManager : MonoBehaviour
    {
        [SerializeField] private CharacterStats BaseCharacterStats;
        public CharacterStats CurrentCharacterStats { get; private set; }

        public bool IsDead { get; private set; }

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {
            CurrentCharacterStats = Instantiate(BaseCharacterStats);
        }

        protected virtual void OnDestroy()
        {
            CurrentCharacterStats = null;
        }
    }
}
