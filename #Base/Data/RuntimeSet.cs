using System.Collections.Generic;
using UnityEngine;

namespace SDE.Data
{
    public interface IRuntime { }

    [CreateAssetMenu(fileName = "RuntimeSet", menuName = "SDE/Data/RuntimeSet")]
    public class RuntimeSet : ScriptableObject
    {
        LinkedList<IRuntime> mList = new LinkedList<IRuntime>();

        // Setters
        public void Add(IRuntime item)
        {
            mList.AddLast(item);
        }
        public void Remove(IRuntime item)
        {
            mList.Remove(item);
        }

        // Getters
        public T GetFirst<T>() where T : IRuntime
        {
            return (T)mList.First.Value;
        }
        public T GetLast<T>() where T : IRuntime
        {
            return (T)mList.Last.Value;
        }
        
        public bool TryGetFirst<T>(ref T result) where T : IRuntime
        {
            return TryGet<T>(ref result, GetFirst<T>);
        }
        public bool TryGetLast<T>(ref T result) where T : IRuntime
        {
            return TryGet<T>(ref result, GetLast<T>);
        }
        
        public void TryApplyToFirst<T>(System.Action<T> action) where T : IRuntime
        {            
            if (IsEmpty)
                return;
            action(GetFirst<T>());
        }

        public bool IsEmpty => mList.Count < 1;
        public LinkedList<IRuntime> List => mList;

        private bool TryGet<T>(ref T result, System.Func<T> fetchMethod) where T : IRuntime
        {
            if (IsEmpty)
                return false;

            result = fetchMethod();
            return result != null;
        }
    }
}