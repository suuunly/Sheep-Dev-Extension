namespace SDE
{
    using Data;
    using UnityEngine;

    public class GenericRuntimeSetter : MonoBehaviour, IRuntime
    {
        public RuntimeSet Set;

        private void Awake()
        {
            Set.Add(this);
        }

        private void OnDestroy()
        {
            Set.Remove(this);
        }
    }
}