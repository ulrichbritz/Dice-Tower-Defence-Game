using UnityEngine;

namespace UB
{
    public class PlayerEffectsManager : CharacterEffectsManager
    {
        #if UNITY_EDITOR
        [Header("Debug Test Effect")]
        [SerializeField] private InstantCharacterEffect instantEffectToTest;
        [SerializeField] private bool processEffect = false;
        #endif

        protected override void Update()
        {
            #if UNITY_EDITOR
            if (processEffect) {
                processEffect = false;
                InstantCharacterEffect effect = Instantiate(instantEffectToTest);
                ProcessInstantCharacterEffect(effect);
            }
            #endif
        }
    }
}
