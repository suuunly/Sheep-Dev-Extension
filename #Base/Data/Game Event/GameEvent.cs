using System.Collections.Generic;
using UnityEngine;

namespace SDE.Data
{

    [CreateAssetMenu(fileName = "GameEvent.asset", menuName = "SDE/Data/Game Event")]
    public class GameEvent : ScriptableObject
    {
        private readonly List<GameEventListener> mEventListners = new List<GameEventListener>();

        public void Raise()
        {
            for (int i = mEventListners.Count - 1; i >= 0; i--)
                mEventListners[i].OnEventRaised();
        }

        public void RegisterListener(GameEventListener listener)
        {
            if (!mEventListners.Contains(listener))
                mEventListners.Add(listener);
        }

        public void UnregisterListener(GameEventListener listener)
        {
            if (mEventListners.Contains(listener))
                mEventListners.Remove(listener);
        }
    }
}