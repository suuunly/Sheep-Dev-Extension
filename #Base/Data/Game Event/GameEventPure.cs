using System;
using UnityEngine;

namespace SDE.Data
{
    [CreateAssetMenu(fileName = "Pure Game Event", menuName = "SDE/Data/Pure Game Event")]
    public class GameEventPure : ScriptableObject
    {
        private void OnDisable()
        {
            OnRaised.RemoveAllListeners();
        }

        public event Action<object> OnRaised;

        public void Raise(object param = null)
        {
            OnRaised.TryInvoke(param);
        }
    }
}