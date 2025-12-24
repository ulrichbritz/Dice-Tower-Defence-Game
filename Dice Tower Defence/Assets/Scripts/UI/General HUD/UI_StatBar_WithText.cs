using UnityEngine;
using TMPro;

namespace UB.UI
{
    public class UI_StatBar_WithText : UI_StatBar
    {
        private TextMeshProUGUI textBox;
        protected override void Awake()
        {
            base.Awake();

            textBox = GetComponentInChildren<TextMeshProUGUI>();
        }

        public override void SetStat(float newValue)
        {
            base.SetStat(newValue);

            textBox.text = $"{slider.value}/{slider.maxValue}";
        }

        public override void SetMaxStat(int maxValue)
        {
            base.SetMaxStat(maxValue);

            textBox.text = $"{slider.value}/{slider.maxValue}";
        }
    }
}

