using UnityEngine;
using AsyncRoutines;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.IO;

namespace UB
{
    /// <summary>
    /// Manages saving and loading of game state
    /// </summary>
    public class WorldSaveGameManager : WorldManager<WorldSaveGameManager>
    {
        public GameObject PlayerPrefab;

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
    }
}