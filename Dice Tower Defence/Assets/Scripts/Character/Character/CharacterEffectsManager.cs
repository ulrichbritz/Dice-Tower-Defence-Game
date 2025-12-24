using UnityEngine;

namespace UB
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        private CharacterManager character;

        protected virtual void Start()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Update()
        {

        }

        public virtual void ProcessInstantCharacterEffect(InstantCharacterEffect effect)
        {
            effect.ProcessEffect(character);
        }
    }
}

