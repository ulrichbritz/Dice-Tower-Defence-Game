using UnityEngine;
using UnityEngine.SceneManagement;

namespace UB.Utilities
{
    public class OnlyEnableMeOnCertainScenes : MonoBehaviour
    {
        [SerializeField] private GameObject ObjectToEnableOrDisable;
        private void Awake()
        {
            ObjectToEnableOrDisable.SetActive(false);
        }

        private void Start()
        {
            SceneManager.activeSceneChanged += OnSceneChange;
        }

        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            // Enable playercontrols in world scene only
            if (newScene.buildIndex == WorldSceneManager.Instance.WorldSceneIndex) {
                ObjectToEnableOrDisable.SetActive(true);
            }
            else {
                ObjectToEnableOrDisable.SetActive(false);
            }
        }
    }
}
