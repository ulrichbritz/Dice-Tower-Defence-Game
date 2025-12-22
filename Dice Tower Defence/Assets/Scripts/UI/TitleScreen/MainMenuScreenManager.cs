using AsyncRoutines;
using UnityEngine;
using UnityEngine.UI;

namespace UB
{
    public class MainMenuScreenManager : MonoBehaviour
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button loadGameButton;
        [SerializeField] private Button quitButton;
        private void Awake()
        {
            newGameButton.Select();
        }

        private void Start()
        {
            newGameButton.onClick.AddListener(OnNewGameButtonClicked);
            loadGameButton.onClick.AddListener(OnLoadGameButtonClicked);
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        private void OnNewGameButtonClicked()
        {
            WorldRoutineManager.Instance.Run(StartNewGame());
        }

        private void OnLoadGameButtonClicked()
        {
            
        }

        private void OnQuitButtonClicked()
        {
            Application.Quit();
        }

        private async Routine StartNewGame()
        {
            Instantiate(WorldSaveGameManager.Instance.PlayerPrefab);
            await WorldSceneManager.Instance.TransitionToSceneAsync(1, true);
            // Show the first wave
            WorldAIManager.Instance.SpawnCharacters(WorldAIManager.Instance.Zombies);
            await RoutineBase.WaitForSeconds(5);
            // Open the in-game shop UI
            PlayerUIManager.Instance.OpenInGameShopMenu();
        }

        private void OnDestroy()
        {
            newGameButton.onClick.RemoveListener(OnNewGameButtonClicked);
            loadGameButton.onClick.RemoveListener(OnLoadGameButtonClicked);
            quitButton.onClick.RemoveListener(OnQuitButtonClicked);
        }
    }
}
