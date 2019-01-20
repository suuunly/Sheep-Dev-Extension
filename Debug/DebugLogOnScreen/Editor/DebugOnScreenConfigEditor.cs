using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DebugOnScreenConfig))]
public class DebugOnScreenConfigEditor : Editor {

    [System.Flags] public enum ApplyTo { Style = 1, Colour  }

    DebugOnScreenConfig mTarget;
    string[] mEnumNames;

    ApplyTo mApplyFlag;

    FontStyle mGlobalStyle = FontStyle.Normal;
    Color mGlobalColour = Color.white;

    private void OnEnable()
    {
        mTarget = (DebugOnScreenConfig)target;
        mEnumNames = System.Enum.GetNames(typeof(LogType));

        EditorUtility.SetDirty(mTarget);
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        DisplayGlobalStyles();
        AddInspectorSpace(5);
        
        DisplayLogStyles();

        AddInspectorSpace(2);
        DisplayMultiEdit();

        if(EditorGUI.EndChangeCheck())
        {
            if (mTarget.DataChangedCallback != null)
                mTarget.DataChangedCallback();
        }

    }

    void DisplayGlobalStyles()
    {
        EditorGUILayout.LabelField("Global Style", EditorStyles.boldLabel);
        mTarget.FontFace = (Font)EditorGUILayout.ObjectField(mTarget.FontFace, typeof(Font), false);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Font Size: ");
        mTarget.FontSize = Mathf.Max(EditorGUILayout.IntField(mTarget.FontSize), 1);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Trace Font Size: ");
        mTarget.TraceFontSize = Mathf.Max(EditorGUILayout.IntField(mTarget.TraceFontSize), 1);
        EditorGUILayout.EndHorizontal();
    }

    void DisplayMultiEdit()
    {
        EditorGUILayout.LabelField("Multi Edit", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
            SetGlobalStyle(ref mGlobalStyle, ref mGlobalColour);
            mApplyFlag = (ApplyTo)EditorGUILayout.EnumFlagsField(mApplyFlag);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Apply To All"))
        {
            foreach (LogStyle style in mTarget.Logs)
            {
                if ((mApplyFlag & ApplyTo.Style) == ApplyTo.Style)
                    style.FontStyle = mGlobalStyle;
                if ((mApplyFlag & ApplyTo.Colour) == ApplyTo.Colour)
                    style.FontColour = mGlobalColour;
            }
        }
    }

    void DisplayLogStyles()
    {
        EditorGUILayout.LabelField("Log Types", EditorStyles.boldLabel);
        for (int i = 0; i < mEnumNames.Length; i++)
        {
            LogStyle style = mTarget.Logs[i];

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                DisplayLogStyleLabels(style, mEnumNames[i]);
                DiplayLogStyle(style);
            EditorGUILayout.EndVertical();
        }
    }

    static void AddInspectorSpace(int amount)
    {
        for (int i = 0; i < amount; i++)
            EditorGUILayout.Space();
    }

    static void DisplayLogStyleLabels(LogStyle style, string name)
    {
        GUIStyle skin = new GUIStyle();
        skin.normal.textColor = style.FontColour;
        skin.fontStyle = style.FontStyle;

        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(name);
            EditorGUILayout.LabelField("(" + name + ")", skin);
        EditorGUILayout.EndHorizontal();
    }

    static void DiplayLogStyle(LogStyle style)
    {
        EditorGUILayout.BeginHorizontal();
            SetGlobalStyle(ref style.FontStyle, ref style.FontColour);
        EditorGUILayout.EndHorizontal();
    }
    static void SetGlobalStyle(ref FontStyle style, ref Color colour)
    {
        style = (FontStyle)EditorGUILayout.EnumPopup(style);
        colour = EditorGUILayout.ColorField(colour);
    }
}
