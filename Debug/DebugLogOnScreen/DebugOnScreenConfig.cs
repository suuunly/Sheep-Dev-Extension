using UnityEngine;

[System.Serializable]
public class LogStyle
{
    public FontStyle FontStyle = FontStyle.Normal;
    public Color FontColour = Color.white;
}

[CreateAssetMenu(fileName ="Log Configuration", menuName = "Debug/On Screen Debug/Log Configuration", order = 1)]
public class DebugOnScreenConfig : ScriptableObject
{
    [HideInInspector] public Font FontFace;
    [HideInInspector] public int FontSize = 12;
    [HideInInspector] public int TraceFontSize;
    [HideInInspector] public LogStyle[] Logs = new LogStyle[System.Enum.GetValues(typeof(LogType)).Length];

    /// <summary>
    /// Will call any callbacks when OnValidate is called on this scriptableObject
    /// </summary>
    [System.NonSerialized] public System.Action DataChangedCallback;

    public LogStyle GetStyle(LogType type)
    {
        return Logs[(int)type];
    }
}
