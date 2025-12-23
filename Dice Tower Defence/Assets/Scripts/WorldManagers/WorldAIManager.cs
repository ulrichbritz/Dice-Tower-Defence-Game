using System.Collections.Generic;
using AsyncRoutines;
using UnityEngine;


namespace UB
{
    public class WorldAIManager : WorldManager<WorldAIManager>
    {
        [Header("AI Characters")]
        public GameObject[] Zombies;
        private List<GameObject> spawnedCharacters = new List<GameObject>();

        [Header("Spawn Settings")]
        [SerializeField] private List<Vector3> enemySpawnPoints = new List<Vector3>();
        [SerializeField] private bool useRandomSpawnPoints = true;
        [SerializeField] private float spawnPointGizmoSize = 1f;

        protected override void Start()
        {
            base.Start();
        }

        public async Routine SpawnCharacters(GameObject[] groupToSpawn)
        {
            for (int i = 0; i < groupToSpawn.Length; i++) {
                var character = groupToSpawn[i];
                Vector3 spawnPosition = GetSpawnPosition();

                // Move camera to spawn position (without returning to normal)
                if (PlayerCameraManager.Instance != null) {
                    PlayerCameraManager.Instance.MoveToPosition(spawnPosition);
                    // Wait for camera to move to position
                    await RoutineBase.WaitForSeconds(0.5f);
                }

                // Spawn the enemy
                GameObject instantiatedCharacter = Instantiate(character, spawnPosition, Quaternion.identity);

                // Make enemy face the origin (0,0,0)
                instantiatedCharacter.transform.LookAt(Vector3.zero);

                spawnedCharacters.Add(instantiatedCharacter);

                // Wait 0.5 seconds after spawning before moving to next spawn
                await RoutineBase.WaitForSeconds(0.5f);
            }

            // After all enemies spawned, return camera to normal position
            if (PlayerCameraManager.Instance != null) {
                PlayerCameraManager.Instance.ReturnToNormalPosition();
            }
        }

        public void DespawnAllCharacters()
        {
            foreach (var character in spawnedCharacters) {
                if (character != null) {
                    Destroy(character);
                }
            }
            spawnedCharacters.Clear();
        }

        /// <summary>
        /// Gets a spawn position based on the configured spawn points
        /// </summary>
        private Vector3 GetSpawnPosition()
        {
            if (enemySpawnPoints == null || enemySpawnPoints.Count == 0) {
                Debug.LogError("No spawn points defined! Spawning at origin.");
                return Vector3.zero;
            }

            if (useRandomSpawnPoints) {
                // Random spawn point selection
                int randomIndex = Random.Range(0, enemySpawnPoints.Count);
                return enemySpawnPoints[randomIndex];
            }
            else {
                // Sequential spawn point selection (cycles through the list)
                int index = spawnedCharacters.Count % enemySpawnPoints.Count;
                return enemySpawnPoints[index];
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Clean up spawned characters
            DespawnAllCharacters();
        }

        #region Spawn Point Generation (Editor Only)
        #if UNITY_EDITOR
        [Header("Spawn Point Generation (Editor Only)")]
        [SerializeField] private Transform[] spawnPlanes = new Transform[0];
        [SerializeField] private int pointsPerPlane = 10;
        [SerializeField] private float edgeBuffer = 2f;
        [SerializeField] private float minDistanceBetweenPoints = 2f;

        /// <summary>
        /// Visualize spawn points in the Scene view
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (enemySpawnPoints != null) {
                for (int i = 0; i < enemySpawnPoints.Count; i++) {
                    // Draw spawn point
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(enemySpawnPoints[i], spawnPointGizmoSize);

                    // Draw spawn point number
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireCube(enemySpawnPoints[i] + Vector3.up * spawnPointGizmoSize, Vector3.one * 0.1f);

                    #if UNITY_EDITOR
                    // Draw index number in scene view
                    UnityEditor.Handles.Label(enemySpawnPoints[i] + Vector3.up * (spawnPointGizmoSize + 0.5f), i.ToString());
                    #endif
                }
            }
        }

        /// <summary>
        /// Generate spawn points from the assigned spawn planes (Editor Only)
        /// Call this method from a custom inspector or context menu
        /// </summary>
        [ContextMenu("Generate Spawn Points From Planes")]
        public void GenerateSpawnPointsFromPlanes()
        {
            if (spawnPlanes == null || spawnPlanes.Length == 0) {
                Debug.LogWarning("No spawn planes assigned!");
                return;
            }

            // Clear existing spawn points
            enemySpawnPoints.Clear();

            foreach (Transform planeTransform in spawnPlanes) {
                if (planeTransform == null) continue;

                GameObject plane = planeTransform.gameObject;

                // Get the plane's bounds (works with MeshRenderer, Collider, etc.)
                Bounds planeBounds = GetObjectBounds(plane);

                if (planeBounds.size == Vector3.zero) {
                    Debug.LogWarning($"Could not get bounds for plane: {plane.name}");
                    continue;
                }

                // Generate random points on this plane, avoiding edges and other spawn points
                int attempts = 0;
                int maxAttempts = pointsPerPlane * 10; // Prevent infinite loops

                for (int i = 0; i < pointsPerPlane && attempts < maxAttempts; attempts++) {
                    Vector3 candidatePoint = GenerateRandomPointOnPlane(planeBounds, planeTransform);

                    // Check if this point is far enough from existing spawn points
                    if (IsValidSpawnPoint(candidatePoint)) {
                        enemySpawnPoints.Add(candidatePoint);
                        i++; // Only increment when we successfully add a point
                    }
                }
            }

            Debug.Log($"Generated {enemySpawnPoints.Count} spawn points from {spawnPlanes.Length} planes.");

            // Mark the scene as dirty so changes are saved
            UnityEditor.EditorUtility.SetDirty(this);
        }

        private Bounds GetObjectBounds(GameObject obj)
        {
            // Try to get bounds from renderer first
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
                return renderer.bounds;

            // Try to get bounds from collider
            Collider collider = obj.GetComponent<Collider>();
            if (collider != null)
                return collider.bounds;

            // Fallback: use transform position with default size
            return new Bounds(obj.transform.position, Vector3.one * 10f);
        }

        private Vector3 GenerateRandomPointOnPlane(Bounds planeBounds, Transform planeTransform)
        {
            // Calculate the usable area (avoiding edges)
            Vector3 min = planeBounds.min;
            Vector3 max = planeBounds.max;

            // Apply edge buffer
            min.x += edgeBuffer;
            min.z += edgeBuffer;
            max.x -= edgeBuffer;
            max.z -= edgeBuffer;

            // Ensure we don't have negative space after applying buffer
            if (min.x >= max.x) min.x = planeBounds.min.x;
            if (min.z >= max.z) min.z = planeBounds.min.z;
            if (min.x >= max.x) max.x = planeBounds.max.x;
            if (min.z >= max.z) max.z = planeBounds.max.z;

            // Generate random point within the buffered area
            Vector3 randomPoint = new Vector3(
                Random.Range(min.x, max.x),
                planeBounds.center.y, // Use the plane's Y position
                Random.Range(min.z, max.z)
            );

            return randomPoint;
        }

        private bool IsValidSpawnPoint(Vector3 candidatePoint)
        {
            // Check distance against all existing spawn points
            foreach (Vector3 existingPoint in enemySpawnPoints) {
                float distance = Vector3.Distance(candidatePoint, existingPoint);
                if (distance < minDistanceBetweenPoints) {
                    return false; // Too close to an existing point
                }
            }
            return true; // Far enough from all existing points
        }
        #endif
        #endregion
    }
}

