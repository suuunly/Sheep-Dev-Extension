using System.Collections.Generic;
using UnityEngine;

namespace SDE.GamePool
{
    [CreateAssetMenu(fileName = "Pooled Data Collection", menuName = "SDE/GamePool/Pooled Data Collections", order = 0)]
    public class PoolData : ScriptableObject
    {
        [System.Serializable]
        public struct Candidate
        {
            public int Amount;
            public GameObject Object;
        }
        public Candidate[] PoolCandidates;

        public void FillDictionary(ref Dictionary<int, GamePool.DataPool> poolDic, System.Action<Candidate> CreateMethod)
        {
            foreach (Candidate candidate in PoolCandidates)
                CreateMethod(candidate);
        }
    }
}


