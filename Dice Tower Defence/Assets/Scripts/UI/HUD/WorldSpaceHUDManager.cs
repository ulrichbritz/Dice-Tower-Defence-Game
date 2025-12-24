using UnityEngine;

namespace UB.UI
{
    public class WorldSpaceHUDManager : MonoBehaviour
    {
        [Header("Facing Camera")]
        [SerializeField] private Transform target;  // character this is on
        [SerializeField] private Vector3 offset;

        [Header("Stat Bars")]
        [SerializeField] private UI_StatBar healthBar;

        private void Start()
        {

        }

        private void Update()
        {
            if (PlayerCameraManager.Instance?.PlayerCamera == null) {
                return;
            }

            transform.rotation = PlayerCameraManager.Instance.PlayerCamera.transform.rotation;
            transform.position = target.position + offset;
        }

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
