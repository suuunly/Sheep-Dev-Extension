using UnityEngine;

namespace SDE
{
    public static class RandomExtensions
    {
        public static float Range01(float max = 100.0f)
        {
            return Random.Range(0.0f, max);
        }
    }
}