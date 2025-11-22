using System;
using AsyncRoutines;
using UnityEngine;

namespace UB
{
    /// <summary>
    /// Lightweight UI animation utility using async routines
    /// Professional-grade tweening without external dependencies
    /// </summary>
    public static class UITweens
    {
        /// <summary>
        /// Scale bounce animation for UI elements
        /// </summary>
        public static async Routine BounceScale(Transform target, float bounceScale = 1.2f, float duration = 0.3f)
        {
            if (target == null) {
                Debug.LogWarning("UITweens.BounceScale: Target transform is null");
                return;
            }
            
            Vector3 originalScale = target.localScale;
            Vector3 targetScale = originalScale * bounceScale;
            float halfDuration = duration * 0.5f;
            
            // Scale up phase
            await ScaleTo(target, targetScale, halfDuration);
            
            // Scale down phase  
            await ScaleTo(target, originalScale, halfDuration);
        }
        
        /// <summary>
        /// Repeating bounce animation that continues until stopped
        /// </summary>
        public static async Routine RepeatingBounce(Transform target, float bounceScale = 1.2f, 
            float bounceDuration = 0.3f, float interval = 2.0f)
        {
            while (true) {
                await RoutineBase.WaitForSeconds(interval);
                await BounceScale(target, bounceScale, bounceDuration);
            }
        }
        
        /// <summary>
        /// Smooth scale transition to target scale
        /// </summary>
        public static async Routine ScaleTo(Transform target, Vector3 targetScale, float duration)
        {
            if (target == null) return;
            
            Vector3 startScale = target.localScale;
            float elapsedTime = 0f;
            
            while (elapsedTime < duration) {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / duration;
                
                // Smooth easing curve
                float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
                target.localScale = Vector3.Lerp(startScale, targetScale, easedProgress);
                
                await RoutineBase.WaitForNextFrame();
            }
            
            // Ensure exact target scale
            target.localScale = targetScale;
        }
        
        /// <summary>
        /// Fade UI element alpha
        /// </summary>
        public static async Routine FadeAlpha(CanvasGroup target, float targetAlpha, float duration)
        {
            if (target == null) {
                Debug.LogWarning("UITweens.FadeAlpha: Target CanvasGroup is null");
                return;
            }
            
            float startAlpha = target.alpha;
            float elapsedTime = 0f;
            
            while (elapsedTime < duration) {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / duration;
                
                float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
                target.alpha = Mathf.Lerp(startAlpha, targetAlpha, easedProgress);
                
                await RoutineBase.WaitForNextFrame();
            }
            
            target.alpha = targetAlpha;
        }
        
        /// <summary>
        /// Fade Image color with automatic GameObject activation handling
        /// </summary>
        public static async Routine FadeImageColor(UnityEngine.UI.Image target, Color targetColor, float duration)
        {
            if (target == null) {
                Debug.LogWarning("UITweens.FadeImageColor: Target Image is null");
                return;
            }
            
            Color startColor = target.color;
            float elapsedTime = 0f;
            
            // Ensure overlay is active for fading
            target.gameObject.SetActive(true);
            
            while (elapsedTime < duration) {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / duration;
                
                // Smooth interpolation
                float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
                target.color = Color.Lerp(startColor, targetColor, easedProgress);
                
                await RoutineBase.WaitForNextFrame();
            }
            
            // Ensure exact target color
            target.color = targetColor;
            
            // Hide overlay if fully transparent
            if (targetColor.a <= 0f) {
                target.gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// Slide UI element to target position
        /// </summary>
        public static async Routine SlideTo(RectTransform target, Vector2 targetPosition, float duration)
        {
            if (target == null) {
                Debug.LogWarning("UITweens.SlideTo: Target RectTransform is null");
                return;
            }
            
            Vector2 startPosition = target.anchoredPosition;
            float elapsedTime = 0f;
            
            while (elapsedTime < duration) {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / duration;
                
                float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
                target.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, easedProgress);
                
                await RoutineBase.WaitForNextFrame();
            }
            
            target.anchoredPosition = targetPosition;
        }
        
        /// <summary>
        /// Pulse animation (scale up and down repeatedly)
        /// </summary>
        public static async Routine Pulse(Transform target, float pulseScale = 1.1f, float pulseDuration = 0.5f)
        {
            if (target == null) return;
            
            Vector3 originalScale = target.localScale;
            
            while (true) {
                await BounceScale(target, pulseScale, pulseDuration);
                await RoutineBase.WaitForSeconds(pulseDuration * 0.5f);
            }
        }
        
        /// <summary>
        /// Shake animation for error feedback or impact
        /// </summary>
        public static async Routine Shake(Transform target, float intensity = 10f, float duration = 0.5f)
        {
            if (target == null) return;
            
            Vector3 originalPosition = target.localPosition;
            float elapsedTime = 0f;
            
            while (elapsedTime < duration) {
                float x = UnityEngine.Random.Range(-intensity, intensity);
                float y = UnityEngine.Random.Range(-intensity, intensity);
                
                target.localPosition = originalPosition + new Vector3(x, y, 0);
                
                elapsedTime += Time.deltaTime;
                await RoutineBase.WaitForNextFrame();
            }
            
            target.localPosition = originalPosition;
        }
        
        /// <summary>
        /// Conditional pulse animation that continues while a condition is met
        /// Perfect for UI button selection states that need to be stoppable
        /// </summary>
        public static async Routine ConditionalPulse(Transform target, System.Func<bool> shouldContinue, 
            float pulseScale = 1.05f, float pulseDuration = 0.3f)
        {
            if (target == null || shouldContinue == null) return;
            
            Vector3 originalScale = target.localScale;
            
            while (shouldContinue())
            {
                // Scale up
                await ScaleTo(target, originalScale * pulseScale, pulseDuration);
                if (!shouldContinue()) break;
                
                // Scale down
                await ScaleTo(target, originalScale, pulseDuration);
                if (!shouldContinue()) break;
            }
            
            // Ensure we return to original scale when stopping
            target.localScale = originalScale;
        }
        
        /// <summary>
        /// Smooth rotation transition to target rotation
        /// </summary>
        public static async Routine RotateTo(Transform target, Vector3 targetRotation, float duration)
        {
            if (target == null) return;
            
            Vector3 startRotation = target.localEulerAngles;
            float elapsedTime = 0f;
            
            while (elapsedTime < duration) {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / duration;
                
                // Smooth easing curve
                float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
                target.localEulerAngles = Vector3.Lerp(startRotation, targetRotation, easedProgress);
                
                await RoutineBase.WaitForNextFrame();
            }
            
            // Ensure exact target rotation
            target.localEulerAngles = targetRotation;
        }
        
        /// <summary>
        /// Conditional wobble animation that continues while a condition is met
        /// Perfect for UI button selection states with rotation feedback
        /// </summary>
        public static async Routine ConditionalWobble(Transform target, System.Func<bool> shouldContinue,
            float maxRotation = 5f, float wobbleDuration = 0.3f)
        {
            if (target == null || shouldContinue == null) return;
            
            Vector3 originalRotation = target.localEulerAngles;
            
            while (shouldContinue())
            {
                // Wobble left
                await RotateTo(target, originalRotation + Vector3.forward * maxRotation, wobbleDuration);
                if (!shouldContinue()) break;
                
                // Wobble right  
                await RotateTo(target, originalRotation - Vector3.forward * maxRotation, wobbleDuration);
                if (!shouldContinue()) break;
            }
            
            // Return to original rotation when stopping
            target.localEulerAngles = originalRotation;
        }
        
        /// <summary>
        /// Conditional color pulse animation for sophisticated UI glow effects
        /// Perfect for card game/roguelike button polish
        /// </summary>
        public static async Routine ConditionalColorPulse(UnityEngine.UI.Image target, System.Func<bool> shouldContinue,
            Color baseColor, Color glowColor, float pulseDuration = 0.5f)
        {
            if (target == null || shouldContinue == null) return;
            
            while (shouldContinue())
            {
                // Glow up
                await ColorTo(target, glowColor, pulseDuration);
                if (!shouldContinue()) break;
                
                // Glow down
                await ColorTo(target, baseColor, pulseDuration);
                if (!shouldContinue()) break;
            }
            
            // Return to base color when stopping
            target.color = baseColor;
        }
        
        /// <summary>
        /// Smooth color transition to target color
        /// </summary>
        public static async Routine ColorTo(UnityEngine.UI.Image target, Color targetColor, float duration)
        {
            if (target == null) return;
            
            Color startColor = target.color;
            float elapsedTime = 0f;
            
            while (elapsedTime < duration) {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / duration;
                
                // Smooth easing curve
                float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
                target.color = Color.Lerp(startColor, targetColor, easedProgress);
                
                await RoutineBase.WaitForNextFrame();
            }
            
            // Ensure exact target color
            target.color = targetColor;
        }
    }
}