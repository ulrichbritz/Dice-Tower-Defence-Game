using UnityEngine;

namespace UB
{
    public class PlayerStatsManager : CharacterStatsManager
    {
        private PlayerManager playerManager;
        [HideInInspector] public PlayerStats CurrentPlayerStats;

        protected override void Awake()
        {
            base.Awake();

            playerManager = GetComponent<PlayerManager>();
            CurrentPlayerStats = (PlayerStats)CurrentCharacterStats;
        }

        protected override void OnHealthChanged(int oldHealth, int newHealth)
        {
            base.OnHealthChanged(oldHealth, newHealth);

            PlayerUIManager.Instance.PlayerUIHudManager.SetNewHealthValue(oldHealth, newHealth);
        }
    }
}
