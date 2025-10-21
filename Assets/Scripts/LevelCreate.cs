using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ColorList
{
    public List<Color> list;
    public ColorList(List<Color> list)
    {
        this.list = list;
    }
}

public class LevelCreate : MonoBehaviour
{
    public LevelController levelController;

    [HorizontalLine(2, EColor.Blue)]
    public int levelId;
    public string levelDiscription;
    public bool isRandom;
    public bool isHardLevel;
    public PictureAsset pictureAsset;
    public List<int> slices;
    [ReadOnly] public List<ColorList> colorsInStage;
    [ReadOnly, ResizableTextArea] public string analyticPicture;

    [HorizontalLine(2, EColor.Blue)]
    [ReadOnly, ResizableTextArea] public string analyticLevel;


    [Button]
    public void SetPictureAsset()
    {
        if (pictureAsset == null)
        {
            Debug.LogError("pictureAsset is Null!");
            return;
        }

        if (slices == null || slices.Count == 0)
        {
            Debug.LogError("slices is Null or Empty!");
            return;
        }

        if (slices.Sum() != 32)
        {
            Debug.LogError("Sum slices not Equals 32!");
            return;
        }

        colorsInStage = new List<ColorList>();

        List<Color> colorList = pictureAsset.Colors.ToList();
        List<Color> pixelColorsInStage = new List<Color>();
        string s = string.Empty;
        for (int i = 1; i < slices.Count + 1; i++)
        {
            var a1 = 32 - slices.Take(i - 1).Sum();
            var a2 = 32 - slices.Take(i).Sum();

            pixelColorsInStage.Clear();
            for (int y = a1 - 1; y >= a2; y--)
            {
                for (int x = 0; x < 32; x++)
                {
                    pixelColorsInStage.Add(colorList[pictureAsset.Data[32 * y + x]]);
                }
            }

            var group = pixelColorsInStage.GroupBy(x => x);

            s = string.Format("{0}\n - slice {1}: {4} | {2} -> {3}", s, i, group.Count(), string.Join(",", group.Select(x => colorList.IndexOf(x.First())).OrderBy(x => x).ToArray()), pixelColorsInStage.Count);
            colorsInStage.Add(new ColorList(group.Select(x => x.First()).ToList()));
        }

        analyticPicture = string.Format("Count Slices: {0}\n {1} ", slices.Count, s);

        Debug.Log("SetPictureAsset Done!");
    }

    [Button]
    public void GenLevel()
    {
#if UNITY_EDITOR
        LevelAsset level = ScriptableObject.CreateInstance<LevelAsset>();

        //spawn
        level.name = string.Format("Level_{0}{1}", levelId, levelDiscription);
        level.PixelData = pictureAsset.GetPixelData();
        level.Slices = slices.ToArray();
        level.CoinsReward = 30;
        level.IsHard = isHardLevel;
        level.HideBoostersUI = false;
        level.DisableRandom = true;

        List<Color> colorList = pictureAsset.Colors.ToList();
        var stageDatas = new List<StageData>();
        string s = string.Empty;
        for (int i = 0; i < slices.Count; i++)
        {
            var stageMap = levelController.stageList[i];
            var stageData = new StageData();

            stageData.Width = stageMap.size.x;
            stageData.Height = stageMap.size.y;

            var tubeElements = new List<TubeElement>();
            var data = new List<StageData.CellData>();
            var listCoil = new List<int>();
            for (int y = 0; y < stageMap.size.y; y++)
            {
                for (int x = 0; x < stageMap.size.x; x++)
                {
                    var dataElement = new StageData.CellData();
                    var dataElementMap = stageMap.map[x, y];

                    dataElement.Type = dataElementMap.GetComponent<ElementCreate>().type;
                    dataElement.Value = dataElementMap.GetComponent<ElementCreate>().value;
                    dataElement.Direction = dataElementMap.GetComponent<ElementCreate>().direction;
                    dataElement.String = string.Empty;

                    if (dataElement.Type == StageData.CellType.Coil || dataElement.Type == StageData.CellType.CoilLocked || dataElement.Type == StageData.CellType.CoilPair)
                    {
                        listCoil.Add(dataElement.Value);
                    }

                    if (dataElement.Type == StageData.CellType.Stack)
                    {
                        var tubeElement = dataElementMap.GetComponentInChildren<TubeElement>();
                        var listColorInTube = tubeElement.coilElementDatas;
                        tubeElements.Add(tubeElement);
                        if (listColorInTube.Count == 0 || listColorInTube.Count != dataElement.Value)
                        {
                            Debug.LogError($"Stage {i} -Stack Error!");
                        }

                        dataElement.String = string.Join(",", listColorInTube);
                        listCoil.AddRange(listColorInTube);
                    }

                    data.Add(dataElement);
                }
            }

            var group = listCoil.GroupBy(x => x).OrderBy(o => o.Key).ToList();
            Debug.Log($"stage {i} - {string.Join(",", group.Select(x => x.Key.ToString()))}");
            if (group.Count != level.PixelData.Colors.Length)
            {
                var colorsInStage = group.Select(x => level.PixelData.Colors[x.Key]).ToList();

                var missingColors = this.colorsInStage[i].list.Except(colorsInStage).ToList();
                var excessColors = colorsInStage.Except(this.colorsInStage[i].list).ToList();
                if (missingColors.Count > 0) Debug.LogError($"Stage {i} - Missing Color: {string.Join(",", missingColors.Select(x => colorList.IndexOf(x).ToString()))}");
                if (excessColors.Count > 0) Debug.LogError($"Stage {i} -Excess Color: {string.Join(",", excessColors.Select(x => colorList.IndexOf(x).ToString()))}");
            }

            foreach (var element in group)
            {
                var count = element.Count() % 3;
                if (count != 0)
                {
                    Debug.LogError($"Stage {i} - Missing Color: {element.Key} -> count: {3 - count}");
                }
            }

            s = string.Format("{0}\n - stage {1}: {2} ->\n{3}", s, i, group.Count(), string.Join("\n", group.Select(x => string.Format("      {0}: {1}", x.Key, x.Count()))));

            stageData.Data = data.ToArray();

            stageData.RandomCoils = string.Join(",", listCoil.OrderBy(x => x).ToArray());
            //Debug.Log(stageData.RandomCoils);
            stageDatas.Add(stageData);
        }

        level.Stages = stageDatas.ToArray();
        analyticLevel = string.Format("Count Stage: {0}\n {1} ", stageDatas.Count, s);


        //save
        string path = string.Format("Assets/Resources/LevelDatas/Level_{0}{1}.asset", levelId, levelDiscription);
        LevelAsset levelAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<LevelAsset>(path);
        if (levelAsset != null)
        {
            // Overwrite the existing asset
            UnityEditor.EditorUtility.CopySerialized(level, levelAsset);
        }
        else
        {
            UnityEditor.AssetDatabase.CreateAsset(level, path);
            UnityEditor.AssetDatabase.SaveAssets();

            UnityEditor.EditorUtility.FocusProjectWindow();

            //UnityEditor.Selection.activeObject = level;
        }

        //save file
        //Debug.Log(JsonUtility.ToJson(level));
        //string pathFileJson = Path.Combine(Application.dataPath, string.Format("Resources/LevelDatas/Level_{0}{1}.txt", levelId, levelDiscription));
        //File.WriteAllText(pathFileJson, JsonUtility.ToJson(level));

        Debug.Log("GenLevel Done!");
#endif
    }
}
