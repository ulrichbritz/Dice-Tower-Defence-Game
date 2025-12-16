using UnityEngine;

namespace UB
{

    public abstract class State : MonoBehaviour
    {
        public abstract State RunCurrentState();
    }
}
