using UnityEngine;
using UnityEngine.SceneManagement;

namespace UB
{
    public class PlayerCameraManager : MonoBehaviour
    {
        public static PlayerCameraManager Instance { get; private set; }

        public Camera PlayerCamera;

        private void Awake()
        {
            if (Instance == null) {
                Instance = this;
            }
            else {
                Destroy(this.gameObject);
            }
        }

        private void Start()
        {
            PlayerCamera = GetComponentInChildren<Camera>();
        }
    }
}
