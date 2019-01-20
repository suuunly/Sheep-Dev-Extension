using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SDE
{
    [Flags]
    public enum EAxis
    {
        X = 1,
        Y = 2,
        Z = 4
    }
    
    
    public static class UnityExtensions
    {
        // ____________________________________
        // @ Vector3
        public static Vector3 SubtractKeepZ(this Vector3 thisV, Vector3 otherV)
        {
            Vector3 newV;
            newV.x = otherV.x - thisV.x;
            newV.y = otherV.y - thisV.y;
            newV.z = thisV.z;
            return newV;
        }

        public static Vector3 SetXY(this Vector3 thisV3, Vector2 otherV2)
        {
            thisV3.x = otherV2.x;
            thisV3.y = otherV2.y;
            
            return thisV3;
        }
        
        // ____________________________________
        // @ Vector2
        public static float Range(this Vector2 v)
        {
            return Random.Range(v.x, v.y);
        }

        // ____________________________________
        // @ Color
        public static float SqrtMagnitude(this Color c)
        {
            return (c.r * c.r + c.g * c.g + c.b * c.b);
        }
        
        // ____________________________________
        // @ Rect
        public static bool ContainsInWorld(this Rect rect, Vector2 worldPos, Vector2 point)
        {
            Vector2 c = rect.center;
            rect.center = worldPos;

            bool contains = rect.Contains(point);
            rect.center = c;
            
            return contains;
        }

        public static Vector2 ClampInWorld(this Rect rect, Vector2 worldPos, Vector2 point)
        {
            Vector2 c = rect.center;
            rect.center = worldPos;

            point.x = Mathf.Clamp(point.x, rect.xMin, rect.xMax);
            point.y = Mathf.Clamp(point.y, rect.yMin, rect.yMax);
            rect.center = c;
            
            return point;
        }
        
        // ____________________________________
        // @ LineRenderer
        public static void ClearPositions(this LineRenderer renderer)
        {
            renderer.positionCount = 0;
        }
        
        // ________________________________________
        // GameObject
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            T comp = go.GetComponent<T>();
            if (!comp) comp = go.AddComponent<T>();
            return comp;
        }

        public static T AddComponent<T>(this GameObject go, System.Action<T> SetProperties) where T : Component
        {
            T comp = go.AddComponent<T>();
            SetProperties(comp);
            return comp;
        }

        public static T TryGetComponentAndApply<T>(this GameObject go, System.Action<T> apply) where T : Component
        {
            T comp = go.GetComponent<T>();
            if (comp)
                apply(comp);
            return comp;
        }
        
        // ________________________________________
        // Transform
        public static void Scale(this Transform t, float x, float y, float z)
        {
            Vector3 scale = t.localScale;
            scale.x += x;
            scale.y += y;
            scale.z += z;
            t.localScale = scale;
        }

        public static void Scale(this Transform t, Vector3 size)
        {
            Scale(t, size.x, size.y, size.z);
        }

        public static void Flip(this Transform t, EAxis direction)
        {
            Vector3 scale = t.localScale;
            
            if (direction.HasFlag(EAxis.X))
                scale.x *= -1.0f;
            if (direction.HasFlag(EAxis.Y))
                scale.y *= -1.0f;
            if (direction.HasFlag(EAxis.Z))
                scale.z *= -1.0f;
            t.localScale = scale;
        }

        public static void SetFlip(this Transform t, bool x, bool y = false, bool z = false)
        {
            Vector3 scale = t.localScale;

            scale.x = x ? scale.x * -1.0f : Mathf.Abs(scale.x);
            scale.y = y ? scale.y * -1.0f : Mathf.Abs(scale.y);
            scale.z = z ? scale.z * -1.0f : Mathf.Abs(scale.z);

            t.localScale = scale;
        }
        
        // _________________________________________
        // @ Array
        public static T RandomValue<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }
        
        // _________________________________________
        // @ List
        public static T RandomValue<T>(this List<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }
    }
}