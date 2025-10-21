using System;
using UnityEngine;

//public class LuckyWheelManager : MonoBehaviour
//{
//#if UNITY_EDITOR
//    [Header("DATA")]
//    [SerializeField] LuckyWheelData luckyWheelData;
//#endif

//    private static LuckyWheelData LuckyWheelData { get; set; }

//    public static Action OnOpen;
//    public static Action<int> OnEarnFreeSpin;
//    public static Action<int> OnSpendFreeSpin;
//    public static Action OnAddSpinCount;
//    public static Action OnResetSpinCount;
//    public static Action OnEarnTicket;

//    private void Awake()
//    {
//        LoadDataJson();

//        OnOpen += Open;
//        OnEarnFreeSpin += EarnFreeSpin;
//        OnSpendFreeSpin += SpendFreeSpin;
//        OnAddSpinCount += AddSpinCount;
//        OnResetSpinCount += ResetSpinCount;
//        OnEarnTicket += EarnTicket;

//        //ActionEvent.OnInitLevelPlay += ResetTicket;
//        //ActionEvent.OnGamePlayWin += SaveDataJson;
//    }

//    private void OnDestroy()
//    {
//        OnOpen -= Open;
//        OnEarnFreeSpin -= EarnFreeSpin;
//        OnSpendFreeSpin -= SpendFreeSpin;
//        OnAddSpinCount -= AddSpinCount;
//        OnResetSpinCount -= ResetSpinCount;
//        OnEarnTicket -= EarnTicket;

//        //ActionEvent.OnInitLevelPlay -= ResetTicket;
//        //ActionEvent.OnGamePlayWin -= SaveDataJson;
//    }

//    private void Open()
//    {
//        PopupLuckyWheel.Instance.Show();
//    }

//    private void EarnFreeSpin(int valueToEarn)
//    {
//        LuckyWheelData.FreeSpin += valueToEarn;
//        SaveDataJson();
//    }

//    private void SpendFreeSpin(int valueToSpend)
//    {
//        LuckyWheelData.FreeSpin -= valueToSpend;
//        SaveDataJson();
//    }

//    private void AddSpinCount()
//    {
//        LuckyWheelData.SpinCount += 1;
//        SaveDataJson();
//    }

//    private void ResetSpinCount()
//    {
//        LuckyWheelData.SpinCount = 0;
//        SaveDataJson();
//    }

//    private void ResetTicket()
//    {
//        LuckyWheelData.TicketInLevel = 0;
//        SaveDataJson();
//    }

//    private void EarnTicket()
//    {
//        LuckyWheelData.TicketInLevel += 3;
//    }

//    public static LuckyWheelData Data => LuckyWheelData;
//    public static int SpinCount => LuckyWheelData.SpinCount;
//    public static int TicketInLevel => LuckyWheelData.TicketInLevel;

//    #region Data Controller
//    private void LoadDataJson()
//    {
//        if (System.IO.File.Exists(DataPath))
//        {
//            string data = System.IO.File.ReadAllText(DataPath);
//            LuckyWheelData = JsonUtility.FromJson<LuckyWheelData>(data);
//        }
//        else LuckyWheelData = new LuckyWheelData();
//#if UNITY_EDITOR
//        luckyWheelData = LuckyWheelData;
//#endif
//    }

//    private void SaveDataJson()
//    {
//        string data = JsonUtility.ToJson(LuckyWheelData);
//        System.IO.File.WriteAllText(DataPath, data);
//    }
//    #endregion
//}
