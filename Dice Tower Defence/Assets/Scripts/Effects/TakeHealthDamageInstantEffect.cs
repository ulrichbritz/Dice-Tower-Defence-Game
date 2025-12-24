using UnityEngine;

namespace UB
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Health Damage Instant Effect")]
    public class TakeHealthDamageInstantEffect : InstantCharacterEffect
    {
        public float HealthDamage;
        public override void ProcessEffect(CharacterManager character)
        {
            CalculcateHealthDamage(character);
        }

        private void CalculcateHealthDamage(CharacterManager character)
        {
            character.CharacterStatsManager.CurrentCharacterStats.CurrentHealth -= Mathf.RoundToInt(HealthDamage);
        }
    }
}
