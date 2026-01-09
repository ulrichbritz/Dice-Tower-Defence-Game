using UnityEngine;

namespace UB
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Health Damage Instant Effect")]
    public class TakeHealthDamageInstantEffect : InstantCharacterEffect
    {
        [Header("Character Causing Damage")]
        public CharacterManager CharacterCausingDamage;

        [Header("Damage")]
        public float physicalDamage;
        // todo maybe add more damage types later
        // todo maybe add build ups like bleed/poison later

        [Header("Final Damage")]
        private int finalDamageDealt;

        [Header("Poise")]
        public float PoiseDamage;
        public bool PoiseIsBroken = false;  // if a characters poise is broken, play stunned anim ("Shield holders enemies maybe"?)

        [Header("Animation")]
        public bool playDamageAnimation = true;
        public bool ManuallySelectDamageAnimation = false;
        public string DamageAnimation;

        [Header("Sound FX")]
        public bool WillPlaySoundEffect = true;
        public AudioClip ElementalDamageSoundFX; // used on top of regular sfx if there is elemental damage (poison, fire, etc)

        [Header("Direction Damage Taken From")]
        public float AngleHitFrom; // used to determine which damage animation to play based on direction hit from
        public Vector3 ContactPoint; // used for hit effects like sparks, blood, etc

        public override void ProcessEffect(CharacterManager character)
        {
            base.ProcessEffect(character);

            if (character.CharacterStatsManager.IsDead) {
                return;
            }

            // todo check for invulnerability

            CalculcateHealthDamage(character);

            // todo check which directional damage came from

            // todo play damage animation

            // todo check for build ups

            // todo play damage sfx

            // todo play vfx (blood etc)
        }

        private void CalculcateHealthDamage(CharacterManager character)
        {
            if (CharacterCausingDamage != null) {
                // todo check for damage modifiers and modify base damage (buffs etc)
                // physical *= physicalModifier etc;
            }

            // todo check character for flat damage reduction (damage resistance etc)

            // add all damage types together and apply final health damage
            finalDamageDealt = Mathf.RoundToInt(physicalDamage);

            if (finalDamageDealt <= 0) {
                finalDamageDealt = 1; // always do at least 1 damage
            }

            character.CharacterStatsManager.CurrentCharacterStats.CurrentHealth -= finalDamageDealt;

            // todo calculate poise damage to determine if poise is broken
        }
    }
}
