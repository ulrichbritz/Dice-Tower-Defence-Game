using System.Collections.Generic;
using UnityEngine;

namespace UB
{
    public class DamageCollider : MonoBehaviour
    {
        [Header("Damage")]
        public float PhysicalDamage;
        // todo add other damage types later (fire, poison, etc)

        [Header("Poise Damage")]
        public float PoiseDamage;

        [Header("Contact Point")]
        public Vector3 ContactPoint;

        [Header("Characters Damaged")]
        protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

        private void OggerEnter(Collider other)
        {
            // check if the object is a character
            CharacterManager damageTarget = other.GetComponent<CharacterManager>();

            if (damageTarget != null) {
                ContactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

                // todo check if we can damage this target (friendly fire)

                // todo check if target invulnerable

                DamageTarget(damageTarget);
            }
        }

        protected virtual void DamageTarget(CharacterManager damageTarget)
        {
            // Dont damage same target more than once in a single attack
            if (charactersDamaged.Contains(damageTarget)) {
                return;
            }

            charactersDamaged.Add(damageTarget);

            TakeHealthDamageInstantEffect damageInstantEffect = Instantiate(WorldCharacterEffectsManager.Instance.TakeHealthDamageInstantEffect);
            damageInstantEffect.physicalDamage = PhysicalDamage;
            damageInstantEffect.ContactPoint = ContactPoint;
            damageInstantEffect.PoiseDamage = PoiseDamage;

            damageTarget.CharacterEffectsManager.ProcessInstantCharacterEffect(damageInstantEffect);
        }
    }
}
