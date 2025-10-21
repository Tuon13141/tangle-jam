using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using LogType = sonat_sdk.Scripts.LogType;

public class UIDebugLog : MonoBehaviour
{
    private static UIDebugLog _instance;
    [SerializeField] private Text txt;
    [SerializeField] private Button close;
    [SerializeField] private bool deactivateOnStart;
    [SerializeField] private int maxLog = 5;
    [SerializeField] private LogType[] acceptedLogs;
    [SerializeField] private Color[] colors;
    private bool[] _unenable;
    private Color[] _inactiveColors;
    private string[] _colorTags;

    [SerializeField] private Image[] filterImgs;
    [SerializeField] private Button[] filterToggle;
    public Button deleteLog;

    public static void Clear()
    {
        Logs.Clear();
    }

    public static IEnumerable<string> GetLogs()
    {
       return Logs.Select(x => x.Mess);
    }
    
    private void Awake()
    {
        _instance = this;
        if (close != null)
            close.onClick.AddListener(() => gameObject.SetActive(false));
        if (deleteLog != null)
            deleteLog.onClick.AddListener(() =>
            {
                txt.text = string.Empty;
                Clear();
            });
        txt.text = "";
        if (deactivateOnStart)
            gameObject.SetActive(false);

        _colorTags = new string[acceptedLogs.Length];
        for (var i = 0; i < acceptedLogs.Length; i++)
            _colorTags[i] = $"<color={ToRgbHex(colors[i])}>{acceptedLogs[i]}</color>: ";

        foreach (var button in filterToggle)
            button.gameObject.SetActive(false);

        _unenable = new bool[acceptedLogs.Length];
        _inactiveColors = colors.Select(x => x * new Color(0.5f, 0.5f, 0.5f, 1f)).ToArray();
        UpdateToggle();

        for (var i = 0; i < acceptedLogs.Length; i++)
        {
            int i1 = i;
            filterToggle[i].gameObject.SetActive(true);
            filterToggle[i].onClick.AddListener(() =>
            {
                _unenable[i1] = !_unenable[i1];
                UpdateToggle();
            });
        }
    }

    private void UpdateToggle()
    {
        for (var i = 0; i < acceptedLogs.Length; i++)
            filterImgs[i].color = _unenable[i] ? _inactiveColors[i] : colors[i];
        UpdateTextLog();
    }


    private static string ToRgbHex(Color c)
    {
        return string.Format("#{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
    }

    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte) (f * 255);
    }

    static readonly List<LogData> Logs = new List<LogData>();


    public static void Log0(string log, bool force = false,LogType logType = LogType.Service)
    {
        if(TestingSettings.DebugLevel <= 0)
            Log(log,force,logType);
    }
    
    public static void Log1(string log, bool force = false,LogType logType = LogType.Service)
    {
        if(TestingSettings.DebugLevel <=  1)
            Log(log,force,logType);
    }
    
    public static void Log2(string log, bool force = false,LogType logType = LogType.Service)
    {
        if(TestingSettings.DebugLevel <=  2)
            Log(log,force,logType);
    }
    
    public static void Log(string log,bool force = false, LogType logType = LogType.Service)
    {
#if UNITY_EDITOR
        if (TestingSettings.UiLogEnable)
            Debug.Log(logType + ":Sonat " + log);
#else
          Debug.Log(logType+":Sonat "+log);
#endif
        if (_instance != null)
            _instance.ShowLog(log, logType);
        else if (force && logType == LogType.Service)
            AddLog(log);
    }

    public static void LogError(string log, bool force = false,LogType logType = LogType.Service)
    {
#if UNITY_EDITOR
        if (TestingSettings.UiLogEnable)
            Debug.LogError(logType + ":Sonat " + log);
#else
//        if (!instanceOnly)
//            Debug.LogError(logType+":Sonat "+log);
#endif
        if (_instance != null)
            _instance.ShowLog(log, logType);
        else if (force && logType == LogType.Service)
            AddLog(log);
    }

    public static void LogWarning(string log, bool force = false, LogType logType = LogType.Service)
    {
#if UNITY_EDITOR
        if (TestingSettings.UiLogEnable)
            Debug.LogWarning(logType + ":Sonat " + log);
#else
//        if (!instanceOnly)
//            Debug.LogError(logType+":Sonat "+log);
#endif
        if (_instance != null)
            _instance.ShowLog(log, logType);
        else if (force && logType == LogType.Service)
            AddLog(log);
    }


    //    public static void Log(object log,bool force, LogType logType = LogType.Service)
    //    {
    //        Log(log.ToString(),force, logType);
    //    }

    private struct LogData
    {
        public readonly int TypeIndex;
        public readonly string Mess;

        public LogData(string mess, int type)
        {
            Mess = mess;
            TypeIndex = type;
        }
    }


    private void ShowLog(string log, LogType logType)
    {
        for (var i = 0; i < acceptedLogs.Length; i++)
        {
            if (acceptedLogs[i] == logType)
            {
                log = $"{_colorTags[i]}{log}";
                Logs.Add(new LogData(log, i));
                if (Logs.Count > _instance.maxLog)
                    Logs.RemoveAt(0);
                UpdateTextLog();
            }
        }
    }

    public static int DefaultMaxLengthOfLog = 50;
    
    private static void AddLog(string log, int type = 1)
    {
        if (_instance == null)
        {
            Logs.Add(new LogData(log, type));
            if (Logs.Count > 50)
            {
                Logs.RemoveAt(0);
            }
        }
    }

    private void UpdateTextLog()
    {
        _instance.txt.text = string.Join(" \n", GetLog());
    }

    private IEnumerable<string> GetLog()
    {
        for (var i = 0; i < Logs.Count; i++)
            if (!_unenable[Logs[i].TypeIndex])
                yield return Logs[i].Mess;
    }
}