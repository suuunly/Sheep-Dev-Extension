using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DebugOnScreen))]
public class DebugOnScreenEditor : Editor
{
    DebugOnScreen mTarget;
    private void OnEnable()
    {
        mTarget = (DebugOnScreen)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Clear"))
            mTarget.Clear();
    }
}
