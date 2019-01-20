using System;
using System.IO;
using UnityEditor;

namespace SDE
{
    using System.Collections;
    using UnityEngine;

    public static class Timing
    {
        public static IEnumerator GenericDelay(float duration, System.Action callback)
        {
            yield return new WaitForSecondsRealtime(duration);
            callback.TryInvoke();
        }

        public static IEnumerator InterpolateTo(Action interpolationMethod, Func<float> distanceMethod,
            System.Action onCallback, float fudgeFactor = 0.2f)
        {
            do
            {
                interpolationMethod();
                yield return null;
            } while (distanceMethod() > fudgeFactor);

            onCallback();
        }
    }

    public static class ObjectExt
    {
        public static TB[] FindObjectsOfType<T, TB>(CSharpExtensions.DelCastingMethod<T, TB> conversionSetter)
            where T : Component
        {
            return Object.FindObjectsOfType<T>().CastTo(conversionSetter);
        }
    }

    #if UNITY_EDITOR
    public static class AssetExt
    {
        public static T CreateAsset<T>(string filename = "") where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string fName = (filename == string.Empty) ? typeof(T).ToString() : filename;

            string assetPathAndName =
                AssetDatabase.GenerateUniqueAssetPath(path + "/" + fName + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            return asset;
        }
    }
    #endif
}
