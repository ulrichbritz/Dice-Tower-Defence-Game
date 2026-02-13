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

            // Start first wave automatically
            CurrentWaveNumber = 0; // Reset to 0 so StartNextWave() makes it wave 1
            await StartNextWave();
        }

        /// <summary>
        /// Starts the next wave in the game
        /// </summary>
        public async Routine StartNextWave()
        {
            // Close shop UI
            PlayerUIManager.Instance.CloseInGameShopMenu();

            // Update wave number
            CurrentWaveNumber++;
            // Set wave in progress flag
            WaveCurrentlyInProgress = true;

            // Spawn enemies for this wave
            await SpawnWaveEnemies();
        }

        /// <summary>
        /// Ends the current wave and opens the shop
        /// </summary>
        public async Routine EndWave()
        {
            if (!WaveCurrentlyInProgress) return; // Already ended

            WaveCurrentlyInProgress = false;

            // Wait a moment then open shop
            await DelayedShopOpen();
        }

        private async Routine SpawnWaveEnemies()
        {
            // Spawn enemies based on wave number - you can customize this logic
            await WorldAIManager.Instance.SpawnCharacters(WorldAIManager.Instance.Zombies);
        }

        private async Routine DelayedShopOpen()
        {
            // Wait a moment before opening shop for dramatic effect
            await RoutineBase.WaitForSeconds(1f);

            // Open the shop for upgrades
            PlayerUIManager.Instance.OpenInGameShopMenu();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
