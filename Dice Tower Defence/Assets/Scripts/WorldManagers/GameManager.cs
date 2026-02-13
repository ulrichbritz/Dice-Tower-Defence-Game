using AsyncRoutines;
using UnityEngine;

namespace UB
{
    /// <summary>
    /// Central game manager for overall game state and logic
    /// </summary>
    public class GameManager : WorldManager<GameManager>
    {
        [Header("Wave Management")]
        public int CurrentWaveNumber { get; set; } = 0;
        public bool WaveCurrentlyInProgress { get; set; }
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        public async Routine StartRun()
        {
            Instantiate(WorldSaveGameManager.Instance.PlayerPrefab);
            await WorldSceneManager.Instance.TransitionToSceneAsync(1, true);
            // Show the first wave
            await WorldAIManager.Instance.SpawnCharacters(WorldAIManager.Instance.Zombies);

            // Wait a few seconds before opening the shop
            await RoutineBase.WaitForSeconds(3f);

            // Open the in-game shop UI
            PlayerUIManager.Instance.OpenInGameShopMenu();
        }

        /// <summary>
        /// Starts the next wave in the game
        /// </summary>
        public void StartNextWave()
        {
            // Update wave number
            CurrentWaveNumber++;
            // Set wave in progress flag
            WaveCurrentlyInProgress = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
