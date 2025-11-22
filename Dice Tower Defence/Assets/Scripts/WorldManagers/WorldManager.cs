using UnityEngine;

namespace UB
{
    /// <summary>
    /// Base class for world manager singletons
    /// </summary>

    public abstract class WorldManager<T> : MonoBehaviour where T : WorldManager<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            CreateInstance();
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

        }

        protected virtual void FixedUpdate()
        {

        }

        protected virtual void LateUpdate()
        {

        }

        protected virtual void OnDestroy()
        {

        }

        private void CreateInstance()
        {
            if (Instance == null) {
                Instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
            }
        }
    }
}