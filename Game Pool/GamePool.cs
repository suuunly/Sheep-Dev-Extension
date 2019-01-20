using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace SDE.GamePool
{
    using Data;

    public class GamePool : MonoBehaviour, IRuntime
    {
        public class DataPool
        {
            public enum EFetchType
            {
                LOOP = 0,
                ONLY_INACTIVE
            }

            private readonly Queue<PooledObjectInstance> mPool;
            private readonly System.Func<PooledObjectInstance>[] mFetchTypeFunc;

            public DataPool(Queue<PooledObjectInstance> pool)
            {
                mPool = pool;
                mFetchTypeFunc = new System.Func<PooledObjectInstance>[] {FetchLoop, FetchOnlyInActive};
            }

            public GameObject Spawn(Vector3 position, Quaternion rotation, EFetchType fetchType)
            {
                return Spawn(inst => inst.Spawn(position, rotation), fetchType);
            }

            public GameObject Spawn(Vector3 position, EFetchType fetchType)
            {
                return Spawn(inst => inst.Spawn(position), fetchType);
            }

            private GameObject Spawn(System.Action<PooledObjectInstance> spawnAction, EFetchType type)
            {
                PooledObjectInstance obj = Fetch(type);
                if (obj == null)
                    return null;

                spawnAction(obj);
                return obj.Instance;
            }

            public PooledObjectInstance Fetch(EFetchType type = EFetchType.LOOP)
            {
                return mFetchTypeFunc[(int) type]();
            }

            private PooledObjectInstance FetchLoop()
            {
                return mPool.DequeueAndEnqueueToBack();
            }

            private PooledObjectInstance FetchOnlyInActive()
            {
                for (int i = 0; i < mPool.Count; i++)
                {
                    PooledObjectInstance value = mPool.DequeueAndEnqueueToBack();
                    if (!value.Instance.activeSelf)
                        return value;
                }

                return null;
            }


            public void DestroyPool()
            {
                while (mPool.Count > 0)
                {
                    PooledObjectInstance obj = mPool.Dequeue();
                    obj.DestroyInstance();
                }
                mPool.Clear();
            }

            public void ResetPool(ResetCriteria resetCriteria)
            {
                for (int i = 0; i < mPool.Count; i++)
                {
                    PooledObjectInstance inst = mPool.DequeueAndEnqueueToBack();
                    if (resetCriteria(inst.Instance))
                        inst.Instance.SetActive(false);
                }
            }

            private static void ValidateInstance<T>(PooledObjectInstance obj, T msg)
            {
                Assert.IsNotNull(obj, "GamePool does not contain: " + msg);
            }
        }

        public class PooledObjectInstance
        {
            public GameObject Instance { get; private set; }
            private readonly System.Action mEvaluateMethod;

            public PooledObjectInstance(GameObject objInstance)
            {
                Instance = objInstance;

                mEvaluateMethod = () => { };

                IPoolable[] poolables = objInstance.GetComponents<IPoolable>();
                // if the object has a ipoolable, set the evaluation method to call the OnSpawned method,
                // otherwise, do nothing
                if (poolables.Length > 0)
                {
                    mEvaluateMethod = null;
                    foreach (IPoolable poolable in poolables)
                    {
                        poolable.OnCreated();
                        mEvaluateMethod += poolable.OnSpawned;
                    }
                }

                Instance.SetActive(false);
            }

            // @ Spawn
            public void Spawn(Vector3 position, Quaternion rotation)
            {
                Instance.transform.rotation = rotation;
                Spawn(position);
            }

            public void Spawn(Vector3 position)
            {
                Instance.transform.position = position;
                Instance.SetActive(true);

                mEvaluateMethod();
            }

            public void DestroyInstance()
            {
                mEvaluateMethod.RemoveAllListeners();
                Destroy(Instance);
            }
        }

        public PoolData PoolDataConfiguration;
        public RuntimeSet GamePoolSet;

        public delegate bool ResetCriteria(GameObject pooledObject);

        private Dictionary<int, DataPool> mPool;

        // ------------------------------------------------
        // @ Mono B
        private void Awake()
        {
            GamePoolSet.Add(this);

            mPool = new Dictionary<int, DataPool>();
            PoolDataConfiguration.FillDictionary(ref mPool, CreatePool);
        }

        private void OnDestroy()
        {
            GamePoolSet.Remove(this);
            foreach (KeyValuePair<int,DataPool> pool in mPool)
                pool.Value.DestroyPool();

            mPool.Clear();
        }

        // ------------------------------------------------
        // @ Pool Managment

        #region Pool Managment

        public void CreatePool(GameObject obj, int amount)
        {
            Queue<PooledObjectInstance> pool = new Queue<PooledObjectInstance>();
            for (int i = 0; i < amount; i++)
            {
                GameObject go = Instantiate(obj);
                go.transform.SetParent(transform);
                pool.Enqueue(new PooledObjectInstance(go));
            }

            mPool.Add(obj.GetInstanceID(), new DataPool(pool));
        }

        public void CreatePool(PoolData.Candidate poolData)
        {
            CreatePool(poolData.Object, poolData.Amount);
        }

        /// <summary>
        /// Will destroy all the objects within the pool entry, and remove any 
        /// reference to the pool
        /// </summary>
        public void RemovePool(int prefabID)
        {
            // if it doesn't exists return
            if (!HasPrefabID(prefabID))
                return;

            // Destroy all of the current pool's objects
            DataPool pool = mPool[prefabID];
            pool.DestroyPool();

            mPool.Remove(prefabID);
        }

        #endregion

        // ------------------------------------------------
        // @ Spawning
        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation,
            DataPool.EFetchType fetchType = DataPool.EFetchType.LOOP)
        {
            DataPool pool = FetchDataPool(prefab);
            return pool.Spawn(position, rotation, fetchType);
        }

        public GameObject Spawn(GameObject prefab, Vector3 position,
            DataPool.EFetchType fetchType = DataPool.EFetchType.LOOP)
        {
            DataPool pool = FetchDataPool(prefab);
            return pool.Spawn(position, fetchType);
        }

        public void ResetPool(string layerMask)
        {
            ResetPool((obj) => obj.layer == LayerMask.NameToLayer(layerMask));
        }

        public void ResetPool(ResetCriteria resetCriteria)
        {
            foreach (KeyValuePair<int, DataPool> pool in mPool)
            {
                DataPool dataPool = pool.Value;
                dataPool.ResetPool(resetCriteria);
            }
        }

        // ----------------------------------------
        // @ Getters
        public bool HasPrefabID(int id)
        {
            return !mPool.ContainsKey(id);
        }

        public PooledObjectInstance FetchObject(GameObject prefab, DataPool.EFetchType type = DataPool.EFetchType.LOOP)
        {
            ValidatePoolContent(prefab);
            return mPool[prefab.GetInstanceID()].Fetch(type);
        }

        public DataPool FetchDataPool(GameObject prefab)
        {
            ValidatePoolContent(prefab);
            return mPool[prefab.GetInstanceID()];
        }

        private void ValidatePoolContent(GameObject prefab)
        {
            Assert.IsTrue(mPool.ContainsKey(prefab.GetInstanceID()),
                "Pool does not contain GameObject: " + prefab.name);
        }
    }
}