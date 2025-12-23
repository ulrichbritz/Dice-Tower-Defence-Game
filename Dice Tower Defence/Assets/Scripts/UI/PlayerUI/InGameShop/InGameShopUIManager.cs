using UnityEngine;
using UnityEngine.UI;

namespace UB
{
    /// <summary>
    /// Manages the Shop UI in the game world
    /// </summary>
    public class InGameShopUIManager : MonoBehaviour
    {
        private PlayerUIManager playerUIManager;
        [SerializeField] private Button nextWaveButton;

        private void Start()
        {
            playerUIManager = PlayerUIManager.Instance;

            nextWaveButton.onClick.AddListener(NextWaveButtonClicked);
        }

        private void NextWaveButtonClicked()
        {
            playerUIManager.CloseInGameShopMenu();
            GameManager.Instance.StartNextWave();
        }

        private void OnDestroy()
        {
            nextWaveButton.onClick.RemoveListener(NextWaveButtonClicked);
        }
    }
}
