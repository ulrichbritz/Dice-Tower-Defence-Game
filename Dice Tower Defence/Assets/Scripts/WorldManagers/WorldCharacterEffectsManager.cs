using System.Collections.Generic;
using UnityEngine;

namespace UB
{
    public class WorldCharacterEffectsManager : WorldManager<WorldCharacterEffectsManager>
    {
        [Header("Damage")]
        public TakeHealthDamageInstantEffect TakeHealthDamageInstantEffect;
        [SerializeField] private List<InstantCharacterEffect> instantCharacterEffectsList = new List<InstantCharacterEffect>();

        protected override void Awake()
        {
            base.Awake();

            GenerateEffectID();
        }

        private void GenerateEffectID()
        {
            for (int i = 0; i < instantCharacterEffectsList.Count; i++) {
                instantCharacterEffectsList[i].InstantEffectID = i;
            }
        }
    }
}
