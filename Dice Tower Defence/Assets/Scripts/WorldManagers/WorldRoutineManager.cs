using UnityEngine;
using AsyncRoutines;

namespace UB
{
    public class WorldRoutineManager : WorldManager<WorldRoutineManager>
    {
        private RoutineManager routineManager;

        protected override void Awake()
        {
            base.Awake();

            routineManager = new RoutineManager();
        }

        protected override void Update()
        {
            base.Update();

            routineManager?.Update();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            routineManager?.Flush();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            routineManager?.StopAll();
        }

        /// <summary>
        /// Run a routine with the global routine manager
        /// </summary>
        public RoutineHandle Run(Routine routine, System.Action<System.Exception> onStop = null)
        {
            return routineManager.Run(routine, onStop);
        }

        /// <summary>
        /// Stop all managed routines
        /// </summary>
        public void StopAll()
        {
            routineManager?.StopAll();
        }

        /// <summary>
        /// Throw an exception in all managed routines
        /// </summary>
        public void ThrowAll(System.Exception exception)
        {
            routineManager?.ThrowAll(exception);
        }
    }
}
