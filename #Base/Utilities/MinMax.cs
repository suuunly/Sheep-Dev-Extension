using System;
using Random = UnityEngine.Random;

namespace SDE
{
    [Serializable]
    public class MinMax
    {
        public float min;
        public float max;

        public float RandomBetween()
        {
            return Random.Range(min, max);
        }
    }
}