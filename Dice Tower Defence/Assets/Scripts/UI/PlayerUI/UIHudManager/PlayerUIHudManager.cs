using UB.UI;
using UnityEngine;

namespace UB
{
    public class PlayerUIHudManager : MonoBehaviour
    {
        [Header("Stat Bars")]
        [SerializeField] private UI_StatBar healthBar;

        public void SetNewHealthValue(float oldValue, float newValue)
        {
            healthBar.SetStat(Mathf.RoundToInt(newValue));
        }

        public void SetMaxHealthValue(int maxHealth)
        {
            healthBar.SetMaxStat(maxHealth);
        }

        private void OnDestroy()
        {

        }
    }
}

