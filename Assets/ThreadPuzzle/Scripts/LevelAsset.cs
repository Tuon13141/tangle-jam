using NaughtyAttributes;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelAsset", menuName = "SOLevel/LevelAsset", order = 0)]
public sealed class LevelAsset : ScriptableObject
{
    [Expandable]
    public PictureAsset PictureAsset;
    public PixelData PixelData;

    [HorizontalLine(2, EColor.Blue)]
    public int[] Slices;

    public StageData[] Stages;

    public TupleSerialize<Color, Color, Color> BackgroundColor;

    public int CoinsReward;

    public bool IsHard;

    public bool HideBoostersUI;

    public bool DisableRandom;

    public LevelAsset()
    {
        Slices = new int[1];
        Stages = new StageData[1];
        DisableRandom = true;
    }

    //[Button]
    public void ConvertPixelData()
    {
        if (PictureAsset == null) return;

        PixelData = new PixelData();
        PixelData.Width = PictureAsset.Width;
        PixelData.Height = PictureAsset.Height;
        PixelData.Colors = PictureAsset.Colors;
        PixelData.Data = PictureAsset.Data;
    }

#if UNITY_EDITOR

    //[Button]
    [ContextMenu("GenDataString")]
    public void GenDataString()
    {
        Debug.Log(JsonUtility.ToJson(this));
        if (!string.IsNullOrEmpty(this.name))
        {
            string pathFileJson = Path.Combine(Application.dataPath, string.Format("Resources/LevelDatas/{0}.txt", this.name));
            File.WriteAllText(pathFileJson, Static.CompressString(JsonUtility.ToJson(this)));
            UnityEditor.AssetDatabase.Refresh();

        }
        else
        {
            Debug.LogError("Name File Error!");
        }
    }

    public string dataString => Static.CompressString(JsonUtility.ToJson(this));

#endif
    //public SliceInfo GetSliceInfo()
    //{
    //	return null;
    //}

    //public int StageCoilsCount(int stageIndex)
    //{
    //	return 0;
    //}

    //public int CoilsCount()
    //{
    //	return 0;
    //}

    //public int StageCountOf(int stageIndex, StageData.CellType cellType)
    //{
    //	return 0;
    //}

    //public int CountOf(StageData.CellType cellType)
    //{
    //	return 0;
    //}
    public enum TutorialType
    {
        None = 0,
        Begin = 1,
        Hidden = 2,
        Stack = 3,
        CoilPair = 4,
        KeyLock = 5,
        PinWall = 6,
    }
    
    public enum Difficulty
    {
        Easy = 0,
        Normal = 1,
        Hard = 2,
        VeryHard = 3,
        Insane = 4,
    }
}
