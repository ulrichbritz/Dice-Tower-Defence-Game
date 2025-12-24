using UnityEngine;
using UnityEngine.UI;

namespace UB.UI
{
    public class UI_StatBar : MonoBehaviour
    {
        protected Slider slider;

        // todo Secondary bar to show recent gains/losses

        protected virtual void Awake()
        {
            slider = GetComponent<Slider>();
        }

        public virtual void SetStat(float newValue)
        {
            slider.value = newValue;
        }

        public virtual void SetMaxStat(int maxValue)
        {
            slider.maxValue = maxValue;
        }
    }
}

