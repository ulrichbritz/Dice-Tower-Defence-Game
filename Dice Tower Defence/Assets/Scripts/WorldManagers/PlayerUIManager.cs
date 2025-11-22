using UnityEngine;
using UnityEngine.UI;

namespace UB
{
    /// <summary>
    /// Manages the player's UI elements in the game world
    /// </summary>
    public class PlayerUIManager : WorldManager<PlayerUIManager>
    {
        [Header("In Game Shop UI")]
        [SerializeField] private GameObject inGameShopUI;
        [SerializeField] private Button nextWaveButton;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            nextWaveButton.onClick.AddListener(NextWaveButtonClicked);
        }

        protected override void Update()
        {
            base.Update();
        }

        # region In Game Shop UI Methods and Functionality

        public void OpenInGameShopMenu()
        {
            inGameShopUI.SetActive(true);
        }

        public void CloseInGameShopMenu()
        {
            inGameShopUI.SetActive(false);
        }

        private void NextWaveButtonClicked()
        {
            CloseInGameShopMenu();
            //WorldWaveManager.Instance.StartNextWave();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            nextWaveButton.onClick.RemoveListener(NextWaveButtonClicked);
        }

        # endregion
    }
}

