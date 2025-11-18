using System;
using System.Collections.Generic;
using AsyncRoutines;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UB
{
    public class WorldSceneManager : WorldManager<WorldSceneManager>
    {
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
        /// Load a new scene and unload the previous one (smooth transition)
        /// </summary>
        public async Routine TransitionToSceneAsync(string newSceneName, string sceneToUnload = null, bool useFadeTransition = false)
        {
            if (useFadeTransition && fadeOverlay != null) {
                // Phase 1: Start fade out and scene loading simultaneously
                var fadeOutTask = UITweens.FadeImageColor(fadeOverlay, Color.black, fadeTransitionTime);
                var sceneLoadTask = LoadSceneAdditiveAsync(newSceneName);
                
                // Wait for both fade out AND scene loading to complete
                await fadeOutTask;
                await sceneLoadTask;
                
                // Phase 2: Set new scene as active
                var newScene = SceneManager.GetSceneByName(newSceneName);
                if (newScene.isLoaded) {
                    SceneManager.SetActiveScene(newScene);
                }
                
                // Phase 3: Unload old scene if specified
                if (!string.IsNullOrEmpty(sceneToUnload)) {
                    await UnloadSceneAsync(sceneToUnload);
                }
                
                // Phase 4: Fade in (minimum time guaranteed by fadeTransitionTime)
                await UITweens.FadeImageColor(fadeOverlay, Color.clear, fadeTransitionTime);
            }
            else {
                // No fade transition - use original logic
                await LoadSceneAdditiveAsync(newSceneName);
                
                var newScene = SceneManager.GetSceneByName(newSceneName);
                if (newScene.isLoaded) {
                    SceneManager.SetActiveScene(newScene);
                }
                
                if (!string.IsNullOrEmpty(sceneToUnload)) {
                    await UnloadSceneAsync(sceneToUnload);
                }
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