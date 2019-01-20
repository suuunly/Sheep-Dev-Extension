using UnityEngine;
using UnityEngine.Assertions;

namespace SDE.Data
{
    public abstract class Variable<T> : ScriptableObject
    {
        public T Value;
    }
}