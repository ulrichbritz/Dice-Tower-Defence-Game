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
            await GameManager.Instance.StartRun();
        }

        private void OnDestroy()
        {
            newGameButton.onClick.RemoveListener(OnNewGameButtonClicked);
            loadGameButton.onClick.RemoveListener(OnLoadGameButtonClicked);
            quitButton.onClick.RemoveListener(OnQuitButtonClicked);
        }
    }
}
