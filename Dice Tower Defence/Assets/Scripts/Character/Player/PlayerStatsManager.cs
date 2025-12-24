using UnityEngine;

namespace UB
{
    public class PlayerStatsManager : CharacterStatsManager
    {
        protected override void OnHealthChanged(int oldHealth, int newHealth)
        {
            base.OnHealthChanged(oldHealth, newHealth);

            PlayerUIManager.Instance.PlayerUIHudManager.SetNewHealthValue(oldHealth, newHealth);
        }
    }
}
