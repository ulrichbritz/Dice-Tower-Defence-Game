using System;
using System.Collections.Generic;
using AsyncRoutines;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UB
{
    /// <summary>
    /// Manages scene loading and unloading in the game world
    /// </summary>
    public class WorldSceneManager : WorldManager<WorldSceneManager>
    {
        public int WorldSceneIndex { get; private set; } = 1;// The build index of the main world scene

        [Header("Scene Loading Settings")]
        [SerializeField] private float minimumLoadTime = 1.0f; // Prevent flashing on fast loads
        [SerializeField] private Image fadeOverlay; // UI overlay for scene transitions
        [SerializeField] private float fadeTransitionTime = 0.5f;
        
        // Events for loading feedback
        public static event Action<string, float> OnSceneLoadProgress;
        public static event Action<string> OnSceneLoadStarted;
        public static event Action<string> OnSceneLoadCompleted;
        public static event Action<string> OnSceneUnloadCompleted;

        // Track loaded scenes
        private readonly HashSet<string> loadedAdditiveScenes = new HashSet<string>();
        
        /// <summary>
        /// Load a scene asynchronously (replaces current scene)
        /// </summary>
        public async Routine LoadSceneAsync(string sceneName)
        {
            OnSceneLoadStarted?.Invoke(sceneName);
            
            var startTime = Time.time;
            var operation = SceneManager.LoadSceneAsync(sceneName);
            
            // Monitor progress
            while (!operation.isDone) {
                OnSceneLoadProgress?.Invoke(sceneName, operation.progress);
                await RoutineBase.WaitForNextFrame();
            }
            
            // Ensure minimum load time for UX
            await WaitForMinimumLoadTime(startTime);
            
            OnSceneLoadCompleted?.Invoke(sceneName);
        }
        
        /// <summary>
        /// Load a scene asynchronously by build index (replaces current scene)
        /// </summary>
        public async Routine LoadSceneAsync(int sceneIndex)
        {
            string sceneName = GetSceneNameFromIndex(sceneIndex);
            OnSceneLoadStarted?.Invoke(sceneName);
            
            var startTime = Time.time;
            var operation = SceneManager.LoadSceneAsync(sceneIndex);
            
            // Monitor progress
            while (!operation.isDone) {
                OnSceneLoadProgress?.Invoke(sceneName, operation.progress);
                await RoutineBase.WaitForNextFrame();
            }
            
            // Ensure minimum load time for UX
            await WaitForMinimumLoadTime(startTime);
            
            OnSceneLoadCompleted?.Invoke(sceneName);
        }
        
        /// <summary>
        /// Load a scene additively (keeps current scene loaded)
        /// </summary>
        public async Routine LoadSceneAdditiveAsync(string sceneName)
        {
            if (loadedAdditiveScenes.Contains(sceneName)) {
                Debug.LogWarning($"Scene '{sceneName}' is already loaded additively");
                return;
            }
            
            OnSceneLoadStarted?.Invoke(sceneName);
            
            var startTime = Time.time;
            var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            
            // Monitor progress
            while (!operation.isDone)
            {
                OnSceneLoadProgress?.Invoke(sceneName, operation.progress);
                await RoutineBase.WaitForNextFrame();
            }
            
            // Ensure minimum load time for UX
            await WaitForMinimumLoadTime(startTime);
            
            loadedAdditiveScenes.Add(sceneName);
            OnSceneLoadCompleted?.Invoke(sceneName);
        }
        
        /// <summary>
        /// Load a scene additively by build index (keeps current scene loaded)
        /// </summary>
        public async Routine LoadSceneAdditiveAsync(int sceneIndex)
        {
            string sceneName = GetSceneNameFromIndex(sceneIndex);
            
            if (loadedAdditiveScenes.Contains(sceneName)) {
                Debug.LogWarning($"Scene '{sceneName}' is already loaded additively");
                return;
            }
            
            OnSceneLoadStarted?.Invoke(sceneName);
            
            var startTime = Time.time;
            var operation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            
            // Monitor progress
            while (!operation.isDone)
            {
                OnSceneLoadProgress?.Invoke(sceneName, operation.progress);
                await RoutineBase.WaitForNextFrame();
            }
            
            // Ensure minimum load time for UX
            await WaitForMinimumLoadTime(startTime);
            
            loadedAdditiveScenes.Add(sceneName);
            OnSceneLoadCompleted?.Invoke(sceneName);
        }
        
        /// <summary>
        /// Unload an additively loaded scene
        /// </summary>
        public async Routine UnloadSceneAsync(string sceneName)
        {
            if (!loadedAdditiveScenes.Contains(sceneName)) {
                Debug.LogWarning($"Scene '{sceneName}' is not loaded additively");
                return;
            }
            
            var operation = SceneManager.UnloadSceneAsync(sceneName);
            
            while (!operation.isDone) {
                await RoutineBase.WaitForNextFrame();
            }
            
            loadedAdditiveScenes.Remove(sceneName);
            OnSceneUnloadCompleted?.Invoke(sceneName);
        }
        
        /// <summary>
        /// Load a new scene by index and unload the previous one (smooth transition)
        /// </summary>
        public async Routine TransitionToSceneAsync(int sceneIndex, bool useFadeTransition = false)
        {
            if (useFadeTransition && fadeOverlay != null) {
                // Phase 1: Fade out
                await UITweens.FadeImageColor(fadeOverlay, Color.black, fadeTransitionTime);
                
                // Phase 2: Load new scene (this automatically unloads the current scene)
                await LoadSceneAsync(sceneIndex);
                
                // Phase 3: Fade in
                await UITweens.FadeImageColor(fadeOverlay, Color.clear, fadeTransitionTime);
            }
            else {
                // No fade transition - direct load
                await LoadSceneAsync(sceneIndex);
            }
        }

        
        /// <summary>
        /// Get all currently loaded additive scenes
        /// </summary>
        public IReadOnlyCollection<string> GetLoadedAdditiveScenes()
        {
            return loadedAdditiveScenes;
        }
        
        /// <summary>
        /// Check if a specific scene is loaded
        /// </summary>
        public bool IsSceneLoaded(string sceneName)
        {
            return SceneManager.GetSceneByName(sceneName).isLoaded;
        }
        
        private async Routine WaitForMinimumLoadTime(float startTime)
        {
            float elapsedTime = Time.time - startTime;
            if (elapsedTime < minimumLoadTime) {
                float remainingTime = minimumLoadTime - elapsedTime;
                await RoutineBase.WaitForSeconds(remainingTime);
            }
        }
        
        /// <summary>
        /// Helper method to get scene name from build index
        /// </summary>
        private string GetSceneNameFromIndex(int sceneIndex)
        {
            if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings) {
                Debug.LogWarning($"Scene index {sceneIndex} is out of range. Available scenes: 0-{SceneManager.sceneCountInBuildSettings - 1}");
                return $"Scene_{sceneIndex}"; // Fallback name
            }
            
            string scenePath = SceneUtility.GetScenePathByBuildIndex(sceneIndex);
            return System.IO.Path.GetFileNameWithoutExtension(scenePath);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            // Clear events to prevent memory leaks
            OnSceneLoadProgress = null;
            OnSceneLoadStarted = null;
            OnSceneLoadCompleted = null;
            OnSceneUnloadCompleted = null;
        }
    }
}