using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OnScreenLog
{
    public string Log { get; private set; }
    public string LogTrace { get; private set; }
    public LogType Type { get; private set; }
    public int NumOfBreakLines { get; private set; }
    public int NumOfTraceBreakLines { get; private set; }

    public OnScreenLog(string log, string logTrace, LogType type) : this()
    {
        Log = log;
        LogTrace = logTrace;
        Type = type;

        NumOfBreakLines = Log.Split('\n').Length; // gets the amount of break lines there are in the log message
        NumOfTraceBreakLines = LogTrace.Split('\n').Length;
    }
}

public class DebugOnScreen : MonoBehaviour
{ 
    // On Screen Placement
    [Header("Log Placement")]
    public Vector2 TopLeft;
    public float MarginTop;

    // Log Style
    [Header("Style")]
    public DebugOnScreenConfig Style;
    public TextAnchor Alignment;

    // Clearing
    [Header("Clearing")]
    public bool ClearOverTime;
    public float TimeBeforeClearing;
    public bool ClearScreenOverflow;
    
    // settings
    [Header("Settings")]
    public int MaxLogs;

    // Trace settings
    [Header("Tracing")]
    public bool ShowTrace = false;
    public float TraceMarginTop;

    private readonly Queue<OnScreenLog> mLogs = new Queue<OnScreenLog>();
    private Rect mBoundingBox = new Rect();

    private readonly GUIStyle mLogStyle = new GUIStyle();
    private readonly GUIStyle mTraceStyle = new GUIStyle();

    private Coroutine mClearCoroutine;

    private float mDirection = 1.0f;

    private const int INTERNAL_TRACE_PADDING = 15;

    // ---------------------------------------------------------
    // @ Mono B
    private void OnValidate()
    {
        MaxLogs = Mathf.Max(1, MaxLogs);
        UpdateTimeClearing(ClearOverTime);

        switch(Alignment)
        {
            case TextAnchor.LowerCenter:
            case TextAnchor.LowerLeft:
            case TextAnchor.LowerRight:
                mDirection = -1.0f;
                break;
            default:
                mDirection = 1.0f;
                break;
        }
    }

    private void OnEnable()
    {
        mBoundingBox.size = new Vector2(Screen.width, Screen.height);

        // Connects to Unity's log message reciever event
        Application.logMessageReceived += LogHandler;
        //Application.logMessageReceivedThreaded += LogHandler;

        // set the clearing state
        UpdateTimeClearing(ClearOverTime);

        // Connects to the Style's OnValidate changes
        Style.DataChangedCallback += UpdateGlobalStyle;
        UpdateGlobalStyle();
    }
    private void OnDisable()
    {
        // Disconnects to Unity's log message reciever event
        Application.logMessageReceived -= LogHandler;
        //Application.logMessageReceivedThreaded -= LogHandler;

        UpdateTimeClearing(false);

        // Disconnects to the Style's OnValidate changes
        Style.DataChangedCallback -= UpdateGlobalStyle;
        mLogs.Clear();
    }

    // ------------------------------------------------
    // @ Update Methods
    public void UpdateGlobalStyle()
    {
        mLogStyle.font = Style.FontFace;
        mLogStyle.fontSize = Style.FontSize;
        mLogStyle.alignment = Alignment;

        mTraceStyle.fontSize = Style.TraceFontSize;
        mTraceStyle.alignment = Alignment;
    }

    void UpdateTimeClearing(bool state)
    {
        if (state)
        {
            if (mClearCoroutine == null)
                mClearCoroutine = StartCoroutine(ClearOverTimeUpdate());
        }
        else if (mClearCoroutine != null)
            StopCoroutine(mClearCoroutine);
    }

    // ---------------------------------------------------------
    // @ Queue Clearing
    public void Clear()
    {
        mLogs.Clear();
    }

    /// <summary>
    /// An update state that repeatedly dequeues Logs over time
    /// </summary>
    /// 
    IEnumerator ClearOverTimeUpdate()
    {
        while(true)
        {
            yield return new WaitForSeconds(TimeBeforeClearing);
            if(mLogs.Count > 1)
                mLogs.Dequeue();
        }
    }

    // ---------------------------------------------------------
    // @ Event Handling
    private void LogHandler(string logString, string stackTrace, LogType type)
    {
        mLogs.Enqueue(new OnScreenLog(logString, stackTrace, type));
        if (mLogs.Count > MaxLogs)
            mLogs.Dequeue();
    }

    // ---------------------------------------------------------
    // @ GUI
    private void OnGUI()
    {
        System.Action showTraceImpl = () => { }; /// creates an empty method by default
        
        int index = 0;

        /// Set the logs start position
        Vector2 pos = TopLeft;
        foreach(OnScreenLog log in mLogs)
        {
            // Set Log style
            LogStyle style = Style.GetStyle(log.Type);
            Color styleColour = style.FontColour;
            mLogStyle.normal.textColor = styleColour;
            mLogStyle.fontStyle = style.FontStyle;

            /// Calculate the log size
            float logScale = Style.FontSize * log.NumOfBreakLines + MarginTop;
            float traceLogScale = 0.0f;

            // If show tracing: allocate to print the trace log
            if (ShowTrace)
            {
                // Set Trace Log Style
                mTraceStyle.normal.textColor = styleColour;

                /// calculate the trace log size
                traceLogScale = Style.TraceFontSize * log.NumOfTraceBreakLines + TraceMarginTop - INTERNAL_TRACE_PADDING;

                /// allocate the print trace log
                showTraceImpl = () =>
                {
                    /// offset the trace by size of the log message (putting the trace log right underneath)
                    pos.y += mDirection * (logScale + TraceMarginTop); 
                    mBoundingBox.position = pos;
                    GUI.Label(mBoundingBox, log.LogTrace, mTraceStyle);
                };
            }

            // Print the Log
            mBoundingBox.position = pos;
            GUI.Label(mBoundingBox, (++index).ToString() + ": " + log.Log, mLogStyle);

            /// this is used to avoid having to call the same if statement twice!
            /// by setting the value to nothing by default, and then in the ONE if statement, 
            /// set it to a new internal method that offsets and prints the trace log
            showTraceImpl();

            /// offset the trace by the number of log messages 
            pos.y += mDirection * (logScale + traceLogScale);
        }

        /// if clear screen overflow is enabled and the position has exceeded the screen size, start dequeing the top
        if (ClearScreenOverflow && pos.y > Screen.height)
            mLogs.Dequeue();
    }

    /// update individual style!
    void UpdateStyle(LogStyle style)
    {
        Color styleColour = style.FontColour;

        // Log Style
        mLogStyle.normal.textColor = styleColour;
        mLogStyle.fontStyle = style.FontStyle;
        mLogStyle.font = Style.FontFace;
        mLogStyle.fontSize = Style.FontSize;

        // Trace Log Style
        mTraceStyle.fontSize = Style.TraceFontSize;
        mTraceStyle.normal.textColor = styleColour;
    }
}
