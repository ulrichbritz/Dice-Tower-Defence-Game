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
