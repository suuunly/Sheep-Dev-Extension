using System.Collections.Generic;

namespace SDE
{
    public static class CSharpExtensions
    {
        // _________________________________________________
        // Delegates
        public delegate bool DelItemComparision<in T>(T item);
        public delegate TB DelCastingMethod<in TA, out TB>(TA original);
        
        // _________________________________________________
        // Actions
        public static void TryInvoke(this System.Action action)
        {
            if (action != null)
                action();
        }

        public static void TryInvoke<T>(this System.Action<T> action, T value)
        {
            if (action != null)
                action(value);
        }

        public static void RemoveAllListeners(this System.Action action)
        {
            if (action == null)
                return;
            
            foreach (System.Delegate del in action.GetInvocationList())
                action -= (System.Action) del;
        }
        
        public static void RemoveAllListeners<T>(this System.Action<T> action)
        {
            if (action == null)
                return;
            
            foreach (System.Delegate del in action.GetInvocationList())
                action -= (System.Action<T>) del;
        }
        

        // _________________________________________________
        // Queues
        /// <summary>
        /// Will dequeue an item and enqueue it again, moving the front most item to the back
        /// </summary>
        /// <returns>Returns the dequeued item</returns>
        public static T DequeueAndEnqueueToBack<T>(this Queue<T> queue)
        {
            T value = queue.Dequeue();
            queue.Enqueue(value);
            return value;
        }
        public static void DequeueAll<T>(this Queue<T> queue, System.Action<T> AfterDequeue)
        {
            while (queue.Count > 0)
                AfterDequeue(queue.Dequeue());
        }

        public static void Enqueue<T>(this Queue<T> queue, T[] array)
        {
            for (int i = 0; i < array.Length; i++)
                queue.Enqueue(array[i]);
        }

        // _________________________________________________
        // Stacks
        public static void PopAll<T>(this Stack<T> stack, System.Action<T> AfterPop)
        {
            while (stack.Count > 0)
                AfterPop(stack.Pop());
        }

        // _________________________________________________
        // Arrays
        public static void Shuffle<T>(this T[] list, System.Random rng)
        {
            int n = list.Length;
            while (n-- > 1)
            {
                int k = rng.Next(n + 1);
                T temp = list[k];
                list[k] = list[n];
                list[n] = temp;
            }
        }
        
        public static T[] ShuffleCopy<T>(this T[] list, System.Random rng)
        {
            T[] newList = new T[list.Length];
            for (int i = 0; i < list.Length; i++)
                newList[i] = list[i];
            
            newList.Shuffle(rng);
            return newList;
        }
        
        
        public static bool Contains<T>(this T[] list, DelItemComparision<T> comparision)
        {
            return Find<T>(list, comparision) != null;

        }
        public static T Find<T>(this T[] list, DelItemComparision<T> comparison)
        {
            foreach (T item in list)
            {
                if (comparison(item))
                    return item;
            }
            return default(T);
        }

        public static void SetAll<T>(this T[] list, System.Action<T> setAction)
        {
            foreach (T item in list)
                setAction(item);
        }
        public static void SetAll<T>(this T[] list, T value) {
            for (int i = 0; i < list.Length; i++)
                list[i] = value;
        }
        public static void SetAll<T>(this T[] list, System.Action<T, int> setAction)
        {
            for (int i = 0; i < list.Length; i++)
                setAction(list[i], i);
        }
        
        public static TB[] CastTo<TA, TB>(this TA[] list, DelCastingMethod<TA,TB> castingMethod)
        {
            int count = list.Length;
            TB[] newList = new TB[count];
            for (int i = 0; i < count; i++)
                newList[i] = castingMethod(list[i]);
            return newList;
        }
        
        public static void Copy<T>(this T[] list, T[] otherList) 
        {
            list = new T[otherList.Length];
            for (int i = 0; i < list.Length; i++)
                list[i] = otherList[i];
        }

        // _________________________________________________
        // Lists
        public static int FindIndex<T>(this List<T> list, DelItemComparision<T> comparison, int failedToFindReturnIndex = -1)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (comparison(list[i]))
                    return i;
            }
            return failedToFindReturnIndex;
        }

        public static TB[] CastToArray<TA, TB>(this List<TA> list, DelCastingMethod<TA,TB> castingMethod) 
        {
            TB[] newList = new TB[list.Count];
            for (int i = 0; i < list.Count; i++)
                newList[i] = castingMethod(list[i]);
            return newList;
        }
        
        // _________________________________________________
        // Linked Lists
        public static T RemoveAndGetFirst<T>(this LinkedList<T> list)
        {
            T value = list.First.Value;
            list.RemoveFirst();
            return value;
        }

        public static int TryRemoveOrAddValue<T>(this LinkedList<T> list, bool shouldRemove, T item)
        {
            if (shouldRemove && !list.Contains(item))
                list.AddLast(item);
            else if (!shouldRemove)
                list.Remove(item);
            return list.Count;
        }

        public static T Find<T>(this LinkedList<T> list, DelItemComparision<T> comparisionMethod, T failedFallback = default(T))
        {
            foreach (T element in list)
            {
                if (comparisionMethod(element))
                    return element;
            }
            return failedFallback;
        }
        
        // _________________________________________________
        // Dictionary
        public static void SetOrAdd<TA, TB>(this Dictionary<TA, TB> dict, TA key, TB value)
        {
            if (dict.ContainsKey(key))
                dict[key] = value;
            else
                dict.Add(key, value);
        }

        public static bool GetAndRemove<TA, TB>(this Dictionary<TA, TB> dict, TA key, ref TB value)
        {
            if (!dict.ContainsKey(key)) 
                return false;
            
            value = dict[key];
            dict.Remove(key);
            return true;
        }

        public static void ApplyToAllAndClear<TA, TB>(this Dictionary<TA, TB> dict, System.Action<TB> apply)
        {
            dict.ApplyToAll(apply);
            dict.Clear();
        }


        public static void ApplyToAll<TA, TB>(this Dictionary<TA, TB> dict, System.Action<TB> apply)
        {
            foreach (KeyValuePair<TA, TB> element in dict)
                apply(element.Value);
        }
    }
}
