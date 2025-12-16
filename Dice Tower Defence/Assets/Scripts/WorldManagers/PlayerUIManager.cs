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
        [HideInInspector]
        public InGameShopUIManager InGameShopUIManager { get; set; }
        [SerializeField] private GameObject inGameShopUI;

        protected override void Awake()
        {
            base.Awake();

            InGameShopUIManager = GetComponentInChildren<InGameShopUIManager>();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        #region In Game Shop UI Methods and Functionality

        public void OpenInGameShopMenu()
        {
            if (inGameShopUI != null) {
                inGameShopUI.SetActive(true);
            }
        }

        public void CloseInGameShopMenu()
        {
            if (inGameShopUI != null) {
                inGameShopUI.SetActive(false);
                Debug.Log("Shop UI closed");
            }
            else {
                Debug.LogWarning("InGameShopUI is not assigned!");
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        #endregion
    }
}

