using UnityEngine;
using AsyncRoutines;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.IO;

namespace UB
{
    public class WorldSaveGameManager : WorldManager<WorldSaveGameManager>
    {
        public int WorldSceneIndex { get; private set; } = 1;// The build index of the main world scene

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