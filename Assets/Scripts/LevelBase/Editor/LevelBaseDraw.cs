using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelAsset))]
[CanEditMultipleObjects]
public class LevelBaseDraw : Editor
{
    private LevelAsset levelBase;

    private int currentStageIndex;
    private Vector2Int currentGridSize;
    private StageData.CellType typeSelect = StageData.CellType.Empty;
    private int valueSelect;
    private StageData.Direction directionSelect;
    private Texture2D texture;

    private int squareSize = 40;
    private int colorSelect = 0;
    private List<ColorList> colorsInStage = new List<ColorList>();
    private string analyticPicture;
    private string analyticLevel;

    private ColorManager colorManager;

    private Texture2D icon_coil;
    private Texture2D icon_coil_pair;
    private Texture2D icon_coil_mystery;
    private Texture2D icon_pin;
    private Texture2D icon_pin_wall;
    private Texture2D icon_stack;
    private Texture2D icon_ribbon_1;
    private Texture2D icon_ribbon_2;
    private Texture2D icon_buttons;
    private Texture2D icon_key;
    private Texture2D icon_lock;

    delegate void DrawTextureCallBack();

    private GUIStyle GetCenteredStyle(int size)
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.normal.textColor = Color.black;
        style.fontSize = size;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold; // Optional: Make text bold
        return style;
    }

    private Dictionary<Color, Texture2D> dicTexture = new Dictionary<Color, Texture2D>();
    public Texture2D MakeTex(Color col)
    {
        if (dicTexture.ContainsKey(col)) return dicTexture[col];

        Texture2D tex = new Texture2D(20, 20);
        Color[] pixels = new Color[tex.width * tex.height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = col;
        }
        tex.SetPixels(pixels);
        tex.Apply();

        dicTexture.Add(col, tex);
        return tex;
    }

    private Dictionary<string, Texture2D> dicTexture1 = new Dictionary<string, Texture2D>();
    private Color[] resultPixelsA;
    private Color[] resultPixelsB;
    Texture2D MergeTwoTextures(Texture2D texA, Color colorA, Texture2D texB = null, Color colorB = default)
    {
        string key = string.Format("{0}_{1}_{2}_{3}", texA.name, colorA.ToString(), texB?.name, colorB.ToString());

        if (dicTexture1.ContainsKey(key)) return dicTexture1[key];
        var result = new Texture2D(texA.width, texA.height, TextureFormat.RGBA32, false);

        resultPixelsA = texA.GetPixels();
        resultPixelsB = texB?.GetPixels();

        for (int i = 0; i < resultPixelsA.Length; i++)
        {
            if (resultPixelsA[i].a == 0) continue;
            resultPixelsA[i] = resultPixelsA[i] * colorA;

            if (resultPixelsB == null || resultPixelsB[i].a == 0) continue;
            resultPixelsA[i] = resultPixelsB[i] * colorB;
        }

        result.SetPixels(resultPixelsA);
        result.Apply(false);

        dicTexture1.Add(key, result);
        return result;
    }

    SerializedProperty sliceList;
    SerializedProperty backgroundColor;
    SerializedProperty isHard;
    SerializedProperty disableRandom;
    private bool hasNewColor;

    private void OnEnable()
    {
        levelBase = (LevelAsset)target;

        sliceList = serializedObject.FindProperty("Slices");
        backgroundColor = serializedObject.FindProperty("BackgroundColor");
        isHard = serializedObject.FindProperty("IsHard");
        disableRandom = serializedObject.FindProperty("DisableRandom");

        icon_coil = Resources.Load<Texture2D>("coil");
        icon_coil_pair = Resources.Load<Texture2D>("coil_pair");
        icon_coil_mystery = Resources.Load<Texture2D>("mystery");
        icon_pin = Resources.Load<Texture2D>("pin");
        icon_pin_wall = Resources.Load<Texture2D>("pin_wall");
        icon_stack = Resources.Load<Texture2D>("stack");
        icon_ribbon_1 = Resources.Load<Texture2D>("ribbon_1");
        icon_ribbon_2 = Resources.Load<Texture2D>("ribbon_2");
        icon_buttons = Resources.Load<Texture2D>("button_stack");
        icon_key = Resources.Load<Texture2D>("key");
        icon_lock = Resources.Load<Texture2D>("lock");

        colorManager = Resources.Load<ColorManager>("ColorManager");

        if (levelBase.Stages == null || levelBase.Slices == null)
        {
            levelBase.Slices = new[] { 32 };
            return;
        }
        if (levelBase.Stages.Length != 0 && currentStageIndex < levelBase.Stages.Length - 1)
        {
            currentGridSize = new Vector2Int(levelBase.Stages[currentStageIndex].Width, levelBase.Stages[currentStageIndex].Height);
        }

        if (levelBase.PixelData?.Colors == null) return;
        if (levelBase.PixelData?.Data == null) return;
        var colorList = levelBase.PixelData.Colors.Select(x => ColorUtility.ToHtmlStringRGB(x)).ToList();
        var colorListCheck = colorManager.coilColorMaterial.Select(x => ColorUtility.ToHtmlStringRGB(x.Value1)).ToList();
        Debug.Log(colorListCheck.Count());

        var check = colorList.Except(colorListCheck).Any();
        if (check)
        {
            hasNewColor = true;
            Debug.LogError("Color Error");
        }
        CheckColor();
    }

    void SetHotKey()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.A)
            {
                directionSelect = StageData.Direction.Left;
                e.Use();
            }
            else if (e.keyCode == KeyCode.D)
            {
                directionSelect = StageData.Direction.Right;
                e.Use();
            }
            else if (e.keyCode == KeyCode.W)
            {
                directionSelect = StageData.Direction.Up;
                e.Use();
            }
            else if (e.keyCode == KeyCode.S)
            {
                directionSelect = StageData.Direction.Down;
                e.Use();
            }
        }
    }

    public override void OnInspectorGUI()
    {
        if (levelBase.Stages == null || levelBase.Slices == null)
        {
            levelBase.Slices = new[] { 32 };
            return;
        }
        SetHotKey();

        serializedObject.Update();

        //DrawDefaultInspector();

        EditorGUILayout.PropertyField(backgroundColor, new GUIContent("BackgroundColor"), true);
        EditorGUILayout.PropertyField(isHard, new GUIContent("isHard"));
        EditorGUILayout.PropertyField(disableRandom, new GUIContent("DisableRandom"));

        EditorGUILayout.PropertyField(sliceList, new GUIContent("Stage"), true);

        serializedObject.ApplyModifiedProperties();

        if (levelBase.Slices.Count() == 0 || levelBase.Slices.Sum() != 32)
        {
            EditorGUILayout.HelpBox("Stage must have the number of elements greater than 0 and the sum of the elements must be equal to 32", MessageType.Error);
        }
        else
        {
            EditorGUILayout.HelpBox("Good Job!", MessageType.Info);
            if (levelBase.Stages.Length != levelBase.Slices.Length)
            {
                var tempStage = new List<StageData>(levelBase.Stages);
                if (levelBase.Stages.Length < levelBase.Slices.Length)
                {
                    for (int i = 0; i < levelBase.Slices.Length - levelBase.Stages.Length; i++)
                    {
                        tempStage.Add(new StageData());
                    }
                }
                else
                {
                    for (int i = 0; i < levelBase.Stages.Length - levelBase.Slices.Length; i++)
                    {
                        tempStage.RemoveAt(levelBase.Stages.Length - 1);
                    }
                }
                currentStageIndex = 0;
                levelBase.Stages = tempStage.ToArray();
            }
        }
        DrawExtension.DrawHorizontalLine();

        ImportPicture();
        DrawExtension.DrawHorizontalLine();
        TopGroup();
        DrawExtension.DrawHorizontalLine();
        SelectDraw();
        DrawExtension.DrawHorizontalLine();

        GUIBlocks();

        GUILayout.Space(6);

        if (GUILayout.Button("CheckLevel"))
        {
            CheckLevel();
        }
        if (GUILayout.Button("RandomColor"))
        {
            RandomColorInCurrentSatage();
        }

        if (hasNewColor && GUILayout.Button("Load New Color"))
        {
            var colorList = levelBase.PixelData.Colors.Select(x => ColorUtility.ToHtmlStringRGB(x)).ToList();
            var colorListCheck = colorManager.coilColorMaterial.Select(x => ColorUtility.ToHtmlStringRGB(x.Value1)).ToList();
            colorManager.ClearNewColors();
            colorManager.newColors = colorList.Except(colorListCheck).Select(x => ColorUtility.TryParseHtmlString("#" + x, out var color) ? color : Color.white).ToList();
            EditorUtility.SetDirty(colorManager);
        }

        EditorGUILayout.HelpBox("Good Job!", MessageType.Info);

        GUI.enabled = false;
        EditorGUILayout.TextArea(analyticLevel);

        UnityEditor.EditorUtility.SetDirty(levelBase);
    }

    private Texture2D oldTexture;
    void ImportPicture()
    {
        texture = (Texture2D)EditorGUILayout.ObjectField("Enter Texture:", texture, typeof(Texture2D), false);
        if (texture == null)
        {
            if (oldTexture != null)
            {
                EditorGUILayout.ObjectField("Old Texture:", oldTexture, typeof(Texture2D), false);

                GUI.enabled = false;
                EditorGUILayout.TextArea(analyticPicture);
                GUI.enabled = true;
            }
            else
            {
                if (levelBase?.PixelData?.Data?.Length > 0)
                {
                    oldTexture = levelBase.PixelData.ToTexture2D();
                }
            }
            return;
        }
        if (oldTexture == null)
        {
            if (levelBase.PixelData.Data.Length > 0)
            {
                oldTexture = levelBase.PixelData.ToTexture2D();
            }
        }

        EditorGUILayout.ObjectField("Old Texture:", oldTexture, typeof(Texture2D), false);

        GUI.enabled = false;
        EditorGUILayout.TextArea(analyticPicture);
        GUI.enabled = true;


        if (GUILayout.Button("Setup Picture"))
        {
            var gridColors = Extract64Colors(texture);
            var grouped = gridColors.GroupBy(x => ColorUtility.ToHtmlStringRGB(x)).ToList();
            Debug.Log(grouped.Count());
            //ColorUtility.ToHtmlStringRGB(color);
            var colorList = grouped.Select(x => x.Key).ToList();
            levelBase.PixelData.Colors = grouped.Select(x => x.First()).ToArray();

            levelBase.PixelData.Data = gridColors.Select(x => ((byte)colorList.IndexOf(ColorUtility.ToHtmlStringRGB(x)))).ToArray();
            // m_GridColorPicker.ApplyGridColors();
            var colorListCheck = colorManager.coilColorMaterial.Select(x => ColorUtility.ToHtmlStringRGB(x.Value1)).ToList();
            Debug.Log(colorListCheck.Count());

            var check = colorList.Except(colorListCheck).Any();
            if (check)
            {
                Debug.LogError("Color Error");
            }

            CheckColor();
            Debug.Log("Gen Done!");
            oldTexture = texture;
        }
    }

    void CheckColor()
    {
        if (levelBase.PixelData?.Colors == null) return;

        colorsInStage = new List<ColorList>();

        List<Color> colorList = levelBase.PixelData.Colors.ToList();
        List<Color> pixelColorsInStage = new List<Color>();
        string s = string.Empty;
        for (int i = 1; i < levelBase.Slices.Length + 1; i++)
        {
            var a1 = 32 - levelBase.Slices.Take(i - 1).Sum();
            var a2 = 32 - levelBase.Slices.Take(i).Sum();

            pixelColorsInStage.Clear();
            for (int y = a1 - 1; y >= a2; y--)
            {
                for (int x = 0; x < 32; x++)
                {
                    pixelColorsInStage.Add(colorList[levelBase.PixelData.Data[32 * y + x]]);
                }
            }

            var group = pixelColorsInStage.GroupBy(x => x);

            s = string.Format("{0}\n - slice {1}: {4} | {2} -> {3}", s, i, group.Count(), string.Join(",", group.Select(x => colorList.IndexOf(x.First())).OrderBy(x => x).ToArray()), pixelColorsInStage.Count);
            colorsInStage.Add(new ColorList(group.Select(x => x.First()).ToList()));
        }

        analyticPicture = string.Format("Count Slices: {0}\n {1} ", levelBase.Slices.Length, s);
    }

    List<Color> Extract64Colors(Texture2D sourceTexture)
    {
        if (sourceTexture == null)
        {
            Debug.LogError("Source texture is not assigned!");
            return null;
        }

        int gridWidth = 32; // Fixed 8x8 grid
        int gridHeight = 32;
        int cellWidth = sourceTexture.width / gridWidth;
        int cellHeight = sourceTexture.height / gridHeight;

        List<Color> colors = new List<Color>();

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Color averageColor = GetAverageColorInCell(sourceTexture, x, y, cellWidth, cellHeight);
                colors.Add(averageColor);
            }
        }

        return colors;
    }

    Color GetAverageColorInCell(Texture2D sourceTexture, int cellX, int cellY, int cellWidth, int cellHeight)
    {
        int startX = cellX * cellWidth;
        int startY = cellY * cellHeight;

        float r = 0, g = 0, b = 0, a = 0;
        int pixelCount = 0;

        // Loop through each pixel in the cell
        for (int y = startY; y < startY + cellHeight; y++)
        {
            for (int x = startX; x < startX + cellWidth; x++)
            {
                if (x >= sourceTexture.width || y >= sourceTexture.height) continue;

                Color pixelColor = sourceTexture.GetPixel(x, y);
                r += pixelColor.r;
                g += pixelColor.g;
                b += pixelColor.b;
                a += pixelColor.a;
                pixelCount++;
            }
        }

        // Return the average color
        return new Color(r / pixelCount, g / pixelCount, b / pixelCount, a / pixelCount);
    }

    void TopGroup()
    {
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("<<<"))
            {
                if (currentStageIndex > 0)
                {
                    currentStageIndex--;
                    currentGridSize = new Vector2Int(levelBase.Stages[currentStageIndex].Width, levelBase.Stages[currentStageIndex].Height);
                }
            }
            GUILayout.Label($"{currentStageIndex + 1}/{levelBase.Slices.Count()}", GUILayout.Width(squareSize));
            if (GUILayout.Button(">>>"))
            {
                if (currentStageIndex < levelBase.Slices.Count() - 1)
                {
                    currentStageIndex++;
                    currentGridSize = new Vector2Int(levelBase.Stages[currentStageIndex].Width, levelBase.Stages[currentStageIndex].Height);
                }
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(15);
        GUILayout.BeginHorizontal();
        {
            if (levelBase.Stages.Length > 0)
            {
                currentGridSize = EditorGUILayout.Vector2IntField(GUIContent.none, currentGridSize);
                if (GUILayout.Button("Gen Grid", GUILayout.Width(squareSize * 2)))
                {
                    Debug.Log($"Gen Grid: {currentGridSize}");
                    levelBase.Stages[currentStageIndex].Width = currentGridSize.x;
                    levelBase.Stages[currentStageIndex].Height = currentGridSize.y;
                    levelBase.Stages[currentStageIndex].Data = new StageData.CellData[currentGridSize.x * currentGridSize.y];
                    for (int i = 0; i < levelBase.Stages[currentStageIndex].Data.Length; i++) { levelBase.Stages[currentStageIndex].Data[i] = new StageData.CellData(); }
                }
            }
        }
        GUILayout.EndHorizontal();

        //EditorGUILayout.Vector2IntField("Size Stage", levelBase.size);
        //type = (StageData.CellType)EditorGUILayout.EnumPopup("Type", type);
        //value = EditorGUILayout.IntField("Value", value);
        //direction = (StageData.Direction)EditorGUILayout.EnumPopup("Direction", direction);
    }

    void GUIBlocks()
    {
        GUILayout.BeginVertical();
        {
            if (levelBase.Stages.Length == 0 || currentStageIndex > levelBase.Stages.Length - 1) return;

            var stage = levelBase.Stages[currentStageIndex];

            Stack<DrawTextureCallBack> callbackDrawTexture = new Stack<DrawTextureCallBack>();

            for (int i = stage.Height - 1; i >= 0; i--)
            {
                GUILayout.BeginHorizontal();
                {
                    for (int j = 0; j < stage.Width; j++)
                    {
                        var index = i * stage.Width + j;
                        var info = stage.Data[index];
                        switch (info.Type)
                        {
                            case StageData.CellType.CoilPair:
                            case StageData.CellType.CoilLocked:
                            case StageData.CellType.Coil:
                                UnityEngine.GUI.backgroundColor = Color.white;
                                if (GUILayout.Button("Coil", GUILayout.Width(squareSize), GUILayout.Height(squareSize)))
                                {
                                    SetupValueGrid(index, info);
                                }

                                var buttonRect = GUILayoutUtility.GetLastRect();
                                callbackDrawTexture.Push(() => DrawTextureCoil(info, buttonRect));
                                break;

                            case StageData.CellType.Stack:
                                UnityEngine.GUI.backgroundColor = Color.white;
                                if (GUILayout.Button("", GUILayout.Width(squareSize), GUILayout.Height(squareSize)))
                                {
                                    SetupValueGrid(index, info);
                                }

                                buttonRect = GUILayoutUtility.GetLastRect();
                                callbackDrawTexture.Push(() => DrawTextureStack(info, buttonRect));
                                break;

                            case StageData.CellType.Wall:
                                UnityEngine.GUI.backgroundColor = Color.gray;
                                if (GUILayout.Button(new GUIContent(info.Type.ToString(), ""), GUILayout.Width(squareSize), GUILayout.Height(squareSize)))
                                {
                                    SetupValueGrid(index, info);

                                }
                                break;

                            case StageData.CellType.PinControl:
                                UnityEngine.GUI.backgroundColor = Color.white;
                                if (GUILayout.Button("", GUILayout.Width(squareSize), GUILayout.Height(squareSize)))
                                {
                                    SetupValueGrid(index, info);
                                }
                                var buttonRect2 = GUILayoutUtility.GetLastRect();
                                DrawTexturePin(info, buttonRect2);
                                break;


                            case StageData.CellType.PinWall:
                                UnityEngine.GUI.backgroundColor = Color.white;
                                if (GUILayout.Button("", GUILayout.Width(squareSize), GUILayout.Height(squareSize)))
                                {
                                    SetupValueGrid(index, info);
                                }
                                var buttonRect3 = GUILayoutUtility.GetLastRect();
                                DrawTexturePin(info, buttonRect3);
                                break;

                            case StageData.CellType.Key:
                                UnityEngine.GUI.backgroundColor = Color.white;
                                if (GUILayout.Button("", GUILayout.Width(squareSize), GUILayout.Height(squareSize)))
                                {
                                    SetupValueGrid(index, info);
                                }
                                var buttonRect4 = GUILayoutUtility.GetLastRect();
                                DrawTextureKey(buttonRect4);
                                break;
                            case StageData.CellType.Lock:
                                UnityEngine.GUI.backgroundColor = Color.white;
                                if (GUILayout.Button("", GUILayout.Width(squareSize), GUILayout.Height(squareSize)))
                                {
                                    SetupValueGrid(index, info);
                                }
                                var buttonRect5 = GUILayoutUtility.GetLastRect();
                                DrawTextureLock(buttonRect5);
                                break;

                            case StageData.CellType.Empty:
                                UnityEngine.GUI.backgroundColor = Color.white;
                                if (GUILayout.Button("", GUILayout.Width(squareSize), GUILayout.Height(squareSize)))
                                {
                                    SetupValueGrid(index, info);
                                }
                                break;

                            case StageData.CellType.ButtonStack:
                                UnityEngine.GUI.backgroundColor = Color.white;
                                if (GUILayout.Button("", GUILayout.Width(squareSize), GUILayout.Height(squareSize)))
                                {
                                    SetupValueGrid(index, info);
                                }

                                buttonRect = GUILayoutUtility.GetLastRect();
                                DrawTextureButtonStack(info, buttonRect);
                                break;

                            default:
                                UnityEngine.GUI.backgroundColor = Color.white;
                                if (GUILayout.Button(new GUIContent(info.Type.ToString(), ""), GUILayout.Width(squareSize), GUILayout.Height(squareSize)))
                                {
                                    SetupValueGrid(index, info);

                                }
                                break;
                        }



                    }
                }

                GUILayout.EndHorizontal();
            }

            while (callbackDrawTexture.Count > 0)
            {
                callbackDrawTexture.Pop().Invoke();
            }
        }

        UnityEngine.GUI.backgroundColor = Color.white;
        GUILayout.EndVertical();
    }

    void SelectDraw()
    {
        if (levelBase?.PixelData?.Colors == null) return;
        EditorGUILayout.BeginHorizontal(); // Start horizontal layout
        {
            for (int i = 0; i < levelBase.PixelData.Colors.Count(); i++)
            {
                Color color = levelBase.PixelData.Colors[i];

                if (colorsInStage[currentStageIndex].list.Contains(color) == false) continue;

                // Display color button
                UnityEngine.GUI.backgroundColor = (colorSelect == i) ? Color.gray : Color.white;
                if (GUILayout.Button(MakeTex(color), GUILayout.Width(squareSize), GUILayout.Height(squareSize)))
                {
                    colorSelect = i;
                }

                GUI.Label(GUILayoutUtility.GetLastRect(), string.Format("{0}", i), GetCenteredStyle(20));
            }

            UnityEngine.GUI.backgroundColor = Color.white;
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(6);

        EditorGUILayout.BeginHorizontal();
        {
            UnityEngine.GUI.backgroundColor = (typeSelect == StageData.CellType.Empty) ? Color.gray : Color.white;
            if (GUILayout.Button("None", GUILayout.Width(squareSize), GUILayout.Height(squareSize))) typeSelect = StageData.CellType.Empty;

            UnityEngine.GUI.backgroundColor = (typeSelect == StageData.CellType.Wall) ? Color.gray : Color.white;
            if (GUILayout.Button("Wall", GUILayout.Width(squareSize), GUILayout.Height(squareSize))) typeSelect = StageData.CellType.Wall;

            UnityEngine.GUI.backgroundColor = (typeSelect == StageData.CellType.Stack) ? Color.gray : Color.white;
            if (GUILayout.Button(icon_stack, GUILayout.Width(squareSize), GUILayout.Height(squareSize))) typeSelect = StageData.CellType.Stack;

            UnityEngine.GUI.backgroundColor = (typeSelect == StageData.CellType.Coil) ? Color.gray : Color.white;
            if (GUILayout.Button(icon_coil, GUILayout.Width(squareSize), GUILayout.Height(squareSize))) typeSelect = StageData.CellType.Coil;

            UnityEngine.GUI.backgroundColor = (typeSelect == StageData.CellType.CoilLocked) ? Color.gray : Color.white;
            if (GUILayout.Button(icon_coil_mystery, GUILayout.Width(squareSize), GUILayout.Height(squareSize))) typeSelect = StageData.CellType.CoilLocked;

            UnityEngine.GUI.backgroundColor = (typeSelect == StageData.CellType.CoilPair) ? Color.gray : Color.white;
            if (GUILayout.Button(icon_coil_pair, GUILayout.Width(squareSize), GUILayout.Height(squareSize))) typeSelect = StageData.CellType.CoilPair;

            UnityEngine.GUI.backgroundColor = (typeSelect == StageData.CellType.PinControl) ? Color.gray : Color.white;
            if (GUILayout.Button(icon_pin, GUILayout.Width(squareSize), GUILayout.Height(squareSize))) typeSelect = StageData.CellType.PinControl;

            UnityEngine.GUI.backgroundColor = (typeSelect == StageData.CellType.PinWall) ? Color.gray : Color.white;
            if (GUILayout.Button(icon_pin_wall, GUILayout.Width(squareSize), GUILayout.Height(squareSize))) typeSelect = StageData.CellType.PinWall;

            UnityEngine.GUI.backgroundColor = (typeSelect == StageData.CellType.ButtonStack) ? Color.gray : Color.white;
            if (GUILayout.Button(icon_buttons, GUILayout.Width(squareSize), GUILayout.Height(squareSize))) typeSelect = StageData.CellType.ButtonStack;

            UnityEngine.GUI.backgroundColor = (typeSelect == StageData.CellType.Lock) ? Color.gray : Color.white;
            if (GUILayout.Button(icon_lock, GUILayout.Width(squareSize), GUILayout.Height(squareSize))) typeSelect = StageData.CellType.Lock;

            UnityEngine.GUI.backgroundColor = (typeSelect == StageData.CellType.Key) ? Color.gray : Color.white;
            if (GUILayout.Button(icon_key, GUILayout.Width(squareSize), GUILayout.Height(squareSize))) typeSelect = StageData.CellType.Key;
        }

        UnityEngine.GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(6);

        EditorGUILayout.BeginHorizontal();
        {
            if (EditorGUILayout.ToggleLeft("Up", directionSelect == StageData.Direction.Up, GUILayout.Width(2 * squareSize), GUILayout.Height(squareSize))) directionSelect = StageData.Direction.Up;
            if (EditorGUILayout.ToggleLeft("Down", directionSelect == StageData.Direction.Down, GUILayout.Width(2 * squareSize), GUILayout.Height(squareSize))) directionSelect = StageData.Direction.Down;
            if (EditorGUILayout.ToggleLeft("Left", directionSelect == StageData.Direction.Left, GUILayout.Width(2 * squareSize), GUILayout.Height(squareSize))) directionSelect = StageData.Direction.Left;
            if (EditorGUILayout.ToggleLeft("Right", directionSelect == StageData.Direction.Right, GUILayout.Width(2 * squareSize), GUILayout.Height(squareSize))) directionSelect = StageData.Direction.Right;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Tutorial");
        var rect = EditorGUILayout.GetControlRect(GUILayout.Width(150), GUILayout.Height(squareSize));
        if (EditorGUI.DropdownButton(rect, new GUIContent(levelBase.Stages[currentStageIndex].tutorialType.ToString()), FocusType.Keyboard))
        {
            var menu = new GenericMenu();
            foreach (LevelAsset.TutorialType type in System.Enum.GetValues(typeof(LevelAsset.TutorialType)))
            {
                menu.AddItem(new GUIContent(type.ToString()), levelBase.Stages[currentStageIndex].tutorialType == type, () =>
                {
                    levelBase.Stages[currentStageIndex].tutorialType = type;
                });
            }
            menu.DropDown(rect);
        }

        EditorGUILayout.LabelField("Difficulty");
        var rect1 = EditorGUILayout.GetControlRect(GUILayout.Width(150), GUILayout.Height(squareSize));
        if (EditorGUI.DropdownButton(rect1, new GUIContent(levelBase.Stages[currentStageIndex].difficulty.ToString()), FocusType.Keyboard))
        {
            var menu = new GenericMenu();
            foreach (LevelAsset.Difficulty type in System.Enum.GetValues(typeof(LevelAsset.Difficulty)))
            {
                menu.AddItem(new GUIContent(type.ToString()), levelBase.Stages[currentStageIndex].difficulty == type, () =>
                {
                    levelBase.Stages[currentStageIndex].difficulty = type;
                });
            }
            menu.DropDown(rect1);
        }
    }

    void SetupValueGrid(int index, StageData.CellData info)
    {
        Undo.RecordObject(levelBase, string.Format("{0}_{1}", levelBase.name, index));

        var stage = levelBase.Stages[currentStageIndex];
        var pos = new Vector2Int(index % stage.Width, index / stage.Width);
        //Debug.Log(JsonUtility.ToJson(info) + " " + pos.ToString());

        if (Event.current.button == 0)
        {
            switch (typeSelect)
            {
                case StageData.CellType.Empty:
                    levelBase.Stages[currentStageIndex].Data[index].Type = StageData.CellType.Empty;
                    break;
                case StageData.CellType.Wall:
                    levelBase.Stages[currentStageIndex].Data[index].Type = StageData.CellType.Wall;
                    break;

                case StageData.CellType.Coil:
                    levelBase.Stages[currentStageIndex].Data[index].Type = StageData.CellType.Coil;
                    levelBase.Stages[currentStageIndex].Data[index].Value = colorSelect;
                    break;

                case StageData.CellType.Stack:
                    levelBase.Stages[currentStageIndex].Data[index].Type = StageData.CellType.Stack;
                    levelBase.Stages[currentStageIndex].Data[index].Direction = directionSelect;
                    break;

                case StageData.CellType.CoilLocked:
                    levelBase.Stages[currentStageIndex].Data[index].Type = StageData.CellType.CoilLocked;
                    levelBase.Stages[currentStageIndex].Data[index].Value = colorSelect;
                    break;

                case StageData.CellType.PinWall:
                    levelBase.Stages[currentStageIndex].Data[index].Type = StageData.CellType.PinWall;
                    levelBase.Stages[currentStageIndex].Data[index].Direction = directionSelect;
                    break;
                case StageData.CellType.PinControl:
                    levelBase.Stages[currentStageIndex].Data[index].Type = StageData.CellType.PinControl;
                    levelBase.Stages[currentStageIndex].Data[index].Direction = directionSelect;
                    break;
                case StageData.CellType.CoilPair:
                    levelBase.Stages[currentStageIndex].Data[index].Type = StageData.CellType.CoilPair;
                    levelBase.Stages[currentStageIndex].Data[index].Value = colorSelect;
                    levelBase.Stages[currentStageIndex].Data[index].Direction = directionSelect;
                    break;

                case StageData.CellType.ButtonStack:
                    levelBase.Stages[currentStageIndex].Data[index].Type = StageData.CellType.ButtonStack;
                    break;
                case StageData.CellType.Key:
                    levelBase.Stages[currentStageIndex].Data[index].Type = StageData.CellType.Key;
                    break;
                case StageData.CellType.Lock:
                    levelBase.Stages[currentStageIndex].Data[index].Type = StageData.CellType.Lock;
                    break;
            }
        }
        else if (Event.current.button == 1)
        {
            //Debug.Log("Right Button Clicked");
            EditorMenuDraw.ShowWindow(info, data => levelBase.Stages[currentStageIndex].Data[index] = data);
        }
    }

    void DrawTextureCoil(StageData.CellData info, Rect buttonRect)
    {
        if (info.Value > levelBase.PixelData.Colors.Length - 1) return;

        Color coilColor = levelBase.PixelData.Colors[info.Value];
        Texture2D texture = MergeTwoTextures(icon_coil, coilColor);

        Vector2 size = new Vector2(texture.width * 0.25f, texture.height * 0.25f);
        Vector2 pivot = buttonRect.center;
        Vector2 pos = pivot - size / 2f;

        GUI.DrawTexture(new Rect(pos, size), texture, ScaleMode.StretchToFill);

        if (info.Type == StageData.CellType.CoilLocked)
        {
            GUI.Label(buttonRect, string.Format("{0}", "?"), GetCenteredStyle(26));
        }

        if (info.Type == StageData.CellType.CoilPair)
        {
            float angle = 0;
            Texture2D textureRibbon = icon_ribbon_1;
            switch (info.Direction)
            {
                case StageData.Direction.Up:
                    textureRibbon = icon_ribbon_2;
                    angle = 90;
                    break;
                case StageData.Direction.Down:
                    angle = 90;
                    break;
                case StageData.Direction.Left:
                    textureRibbon = icon_ribbon_2;
                    //angle = 180;
                    break;
                case StageData.Direction.Right:
                    //angle = 0;
                    break;

                default:
                    break;
            }

            size = new Vector2(texture.width * 0.32f, texture.height * 0.32f);
            pos = pivot - size / 2f;
            GUIUtility.RotateAroundPivot(angle, pivot);
            GUI.DrawTexture(new Rect(pos, size), textureRibbon, ScaleMode.StretchToFill);
            GUIUtility.RotateAroundPivot(-1 * angle, pivot);
        }

        GUI.Label(new Rect(pivot - Vector2.one * 0.75f * squareSize, size), string.Format("{0}", info.MoveIndex), GetCenteredStyle(11));
    }

    void DrawTextureButtonStack(StageData.CellData info, Rect buttonRect)
    {
        var texture = icon_buttons;
        Vector2 size = new Vector2(texture.width * 0.25f, texture.height * 0.25f);
        Vector2 pivot = buttonRect.center;
        Vector2 pos = pivot - size / 2f;

        GUI.DrawTexture(new Rect(pos, size), texture, ScaleMode.StretchToFill);
        GUI.Label(buttonRect, string.Format("{0}", info.Value), GetCenteredStyle(20));

        GUI.Label(new Rect(pivot - Vector2.one * 0.75f * squareSize, size), string.Format("{0}", info.MoveIndex), GetCenteredStyle(11));

    }

    void DrawTexturePin(StageData.CellData info, Rect buttonRect)
    {
        var texture = (info.Type == StageData.CellType.PinControl) ? icon_pin : icon_pin_wall;

        Vector2 size = new Vector2(texture.width * 0.25f, texture.height * 0.25f);
        Vector2 pivot = buttonRect.center;
        Vector2 pos = pivot - size / 2f;

        float angle = 0;
        switch (info.Direction)
        {
            case StageData.Direction.Up:
                angle = 90;
                break;
            case StageData.Direction.Down:
                angle = 270;
                break;
            case StageData.Direction.Left:
                angle = 0;
                break;
            case StageData.Direction.Right:
                angle = 180;
                break;

            default:
                break;
        }

        GUIUtility.RotateAroundPivot(angle, pivot);
        GUI.DrawTexture(new Rect(pos, size), texture, ScaleMode.StretchToFill);
        GUIUtility.RotateAroundPivot(-1 * angle, pivot);
    }

    void DrawTextureStack(StageData.CellData info, Rect buttonRect)
    {
        var texture = icon_stack;
        Vector2 size = new Vector2(texture.width * 0.25f, texture.height * 0.25f);
        Vector2 pivot = buttonRect.center;
        Vector2 pos = pivot - size / 2f;

        float angle = 0;
        switch (info.Direction)
        {
            case StageData.Direction.Up:
                angle = 0;
                break;
            case StageData.Direction.Down:
                angle = 180;
                break;
            case StageData.Direction.Left:
                angle = 270;
                break;
            case StageData.Direction.Right:
                angle = 90;
                break;

            default:
                break;
        }

        GUIUtility.RotateAroundPivot(angle, pivot);
        GUI.DrawTexture(new Rect(pos, size), texture, ScaleMode.StretchToFill);
        GUIUtility.RotateAroundPivot(-1 * angle, pivot);

        GUI.Label(buttonRect, string.Format("{0}", info.Value), GetCenteredStyle(26));

        GUI.Label(new Rect(pivot - Vector2.one * 0.75f * squareSize, size), string.Format("{0}", info.MoveIndex), GetCenteredStyle(11));
    }

    private void DrawTextureKey(Rect buttonRect)
    {
        var iconKey = icon_key;
        var size = new Vector2(iconKey.width * 0.25f, iconKey.height * 0.25f);
        var pivot = buttonRect.center;
        var pos = pivot - size / 2f;
        GUI.DrawTexture(new Rect(pos, size), iconKey, ScaleMode.StretchToFill);
    }

    private void DrawTextureLock(Rect buttonRect)
    {
        var iconLock = icon_lock;
        var size = new Vector2(iconLock.width * 0.25f, iconLock.height * 0.25f);
        var pivot = buttonRect.center;
        var pos = pivot - size / 2f;
        GUI.DrawTexture(new Rect(pos, size), iconLock, ScaleMode.StretchToFill);
    }

    void RandomColorInCurrentSatage()
    {
        var currentStage = levelBase.Stages[currentStageIndex];
        var coilList = new List<StageData.CellData>();
        var coilPos = new List<Vector2Int>();
        for (var i = currentStage.Height - 1; i >= 0; i--)
        {
            for (int j = 0; j < currentStage.Width; j++)
            {
                var index = i * currentStage.Width + j;
                var info = currentStage.Data[index];
                if (info.Type is StageData.CellType.Coil or StageData.CellType.CoilLocked
                    or StageData.CellType.CoilPair)
                {
                    coilList.Add(info);
                    coilPos.Add(new Vector2Int(j, i));
                }
            }
        }
        var colorInStage = coilList.Select(x => x.Value).ToList();
        var colorRandom = Randomize(colorInStage, currentStage.difficulty, coilPos);
        for (var i = 0; i < coilList.Count; i++)
        {
            coilList[i].Value = colorRandom[i];
        }
        EditorUtility.SetDirty(levelBase);
    }
    public List<int> Randomize(List<int> input, LevelAsset.Difficulty difficulty, List<Vector2Int> coilPos)
    {
        // Copy list để không làm thay đổi list gốc
        List<int> list = new List<int>(input);

        // Shuffle ban đầu
        list = list.OrderBy(_ => UnityEngine.Random.Range(0, 10000)).ToList();

        switch (difficulty)
        {
            case LevelAsset.Difficulty.Easy:
                // Gom nhóm các số giống nhau lại gần nhau (sắp xếp tăng dần)
                list = list.OrderBy(x => x).ToList();
                break;

            case LevelAsset.Difficulty.Normal:
                // Giữ nguyên shuffle (random đều)
                break;

            case LevelAsset.Difficulty.Hard:
                list = SpreadOut(list, coilPos, 2); // cách nhau >= 2 ô nếu có thể
                break;

            case LevelAsset.Difficulty.VeryHard:
                list = SpreadOut(list, coilPos, 3);
                break;

            case LevelAsset.Difficulty.Insane:
                list = SpreadOut(list, coilPos, 4);
                break;

        }

        return list;
    }

    private static List<int> SpreadOut(List<int> list, List<Vector2Int> positions, int minDistance)
    {
        var groups = list.GroupBy(x => x)
            .OrderByDescending(g => g.Count()) // nhóm nhiều trước
            .ToList();

        int totalCount = list.Count;
        int[] result = new int[totalCount];
        for (int i = 0; i < result.Length; i++)
            result[i] = int.MinValue;

        // Hàm tính khoảng cách Manhattan
        float Distance(Vector2Int a, Vector2Int b) =>
            Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

        foreach (var g in groups)
        {
            foreach (var value in g)
            {
                int bestIndex = -1;
                float bestScore = float.MinValue;

                for (int i = 0; i < totalCount; i++)
                {
                    if (result[i] != int.MinValue) continue;

                    // Tính khoảng cách tới các ô đã đặt cùng màu
                    float minDistToSame = float.MaxValue;
                    for (int j = 0; j < totalCount; j++)
                    {
                        if (result[j] == value)
                        {
                            float d = Distance(positions[i], positions[j]);
                            if (d < minDistToSame) minDistToSame = d;
                        }
                    }

                    // Nếu chưa có cùng màu nào -> coi như điểm rất tốt
                    if (minDistToSame == float.MaxValue)
                        minDistToSame = 9999f;

                    // Điểm càng cao khi khoảng cách càng lớn
                    float score = minDistToSame;

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestIndex = i;
                    }
                }

                // Nếu không còn chỗ nào thì bỏ vào ô trống bất kỳ
                if (bestIndex == -1)
                    bestIndex = System.Array.FindIndex(result, r => r == int.MinValue);

                result[bestIndex] = value;
            }
        }

        return result.ToList();
    }

    void CheckLevel()
    {
        DrawExtension.ClearConsole();
        List<Color> colorList = levelBase.PixelData.Colors.ToList();
        string s = string.Empty;
        for (int i = 0; i < levelBase.Slices.Length; i++)
        {
            var stageMap = levelBase.Stages[i];

            var tubeElements = new List<TubeElement>();
            var data = new List<StageData.CellData>();
            var listCoil = new List<(int color, Vector2Int pos, int difficulty)>();
            var cointMove = 0;

            //gen map
            int[,] map = new int[stageMap.Width, stageMap.Height];
            for (int y = 0; y < stageMap.Height; y++)
            {
                for (int x = 0; x < stageMap.Width; x++)
                {
                    var dataElement = stageMap.Data[y * stageMap.Width + x];
                    switch (dataElement.Type)
                    {
                        case StageData.CellType.Stack:
                        case StageData.CellType.Wall:
                            map[x, y] = -1;
                            break;

                        case StageData.CellType.Empty:
                            map[x, y] = 0;
                            break;
                        case StageData.CellType.Key:
                        case StageData.CellType.Lock:
                        case StageData.CellType.PinWall:
                        case StageData.CellType.PinControl:
                        case StageData.CellType.Coil:
                        case StageData.CellType.CoilPair:
                            map[x, y] = 1;
                            break;

                        case StageData.CellType.CoilLocked:
                            map[x, y] = 2;
                            break;

                        case StageData.CellType.ButtonStack:
                            map[x, y] = dataElement.Value;
                            break;
                    }
                }
            }

            var stackListPos = new List<Vector2Int>();

            //check coil
            for (int y = 0; y < stageMap.Height; y++)
            {
                for (int x = 0; x < stageMap.Width; x++)
                {
                    var dataElement = stageMap.Data[y * stageMap.Width + x];
                    if (dataElement.Type == StageData.CellType.Coil || dataElement.Type == StageData.CellType.CoilLocked || dataElement.Type == StageData.CellType.CoilPair)
                    {

                        var result = Utils.FindPath(map, new Vector2Int(x, y), out Stack<Vector2Int> stackOut, out int totalCost);
                        if (result)
                        {
                            stageMap.Data[y * stageMap.Width + x].MoveIndex = totalCost;
                            cointMove += totalCost;
                        }

                        listCoil.Add(new(dataElement.Value, new Vector2Int(x, y), totalCost));
                    }

                    if (dataElement.Type == StageData.CellType.Stack)
                    {
                        if (dataElement.Value == 0)
                        {
                            Debug.LogError($"Stage {i} -Stack Error!");
                        }

                        stackListPos.Add(new Vector2Int(x, y));
                    }

                    if (dataElement.Type == StageData.CellType.ButtonStack)
                    {
                        stageMap.Data[y * stageMap.Width + x].MoveIndex = dataElement.Value;
                    }

                    data.Add(dataElement);
                }
            }

            //check stack
            foreach (var stackPos in stackListPos)
            {
                var coilValueTaget = 0;
                var dataElement = stageMap.Data[stackPos.y * stageMap.Width + stackPos.x];
                switch (dataElement.Direction)
                {
                    case StageData.Direction.Up:
                        coilValueTaget = stageMap.Data[(stackPos.y + 1) * stageMap.Width + stackPos.x].MoveIndex + 1;
                        break;
                    case StageData.Direction.Right:
                        coilValueTaget = stageMap.Data[stackPos.y * stageMap.Width + (stackPos.x + 1)].MoveIndex + 1;
                        break;
                    case StageData.Direction.Down:
                        coilValueTaget = stageMap.Data[(stackPos.y - 1) * stageMap.Width + stackPos.x].MoveIndex + 1;
                        break;
                    case StageData.Direction.Left:
                        coilValueTaget = stageMap.Data[stackPos.y * stageMap.Width + (stackPos.x - 1)].MoveIndex + 1;
                        break;
                }

                stageMap.Data[stackPos.y * stageMap.Width + stackPos.x].MoveIndex = coilValueTaget;

                var stackIndex = stageMap.Data[stackPos.y * stageMap.Width + stackPos.x].Value;
                cointMove += (int)(coilValueTaget * stackIndex + Static.TriangularNumber(stackIndex - 1));


                var listColorInTube = dataElement.String.Split(",").Select(x => int.Parse(x)).ToList();
                for (int j = 0; j < listColorInTube.Count; j++)
                {
                    listCoil.Add(new(listColorInTube[j], stackPos, coilValueTaget + j));
                }
            }

            Debug.LogFormat("Difficulty:{0}", (float)cointMove / listCoil.Count);

            var group = listCoil.GroupBy(x => x.color).OrderBy(o => o.Key).ToList();
            Debug.Log($"stage {i} - {string.Join(",", group.Select(x => x.Key.ToString()))}");
            if (group.Count != levelBase.PixelData.Colors.Length)
            {
                var colorsInStage = group.Select(x => levelBase.PixelData.Colors[x.Key]).ToList();

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

            Dictionary<int, List<Vector2Int>> colorGroups = listCoil.GroupBy(e => e.color)
                                                                    .ToDictionary(g => g.Key, g => g.Select(e => e.pos).ToList());

            float totalDistance = 0;
            foreach (var kvp in colorGroups)
            {
                var distanceColor = 0f;
                var countPos = 0;
                var positions = kvp.Value;
                for (int k = 0; k < positions.Count; k++)
                {
                    for (int j = k + 1; j < positions.Count; j++)
                    {
                        distanceColor += Vector2Int.Distance(positions[k], positions[j]);
                        countPos++;
                    }
                }
                totalDistance += distanceColor / countPos;
            }

            var totalDifficulty =
                        0.7f * (float)cointMove / listCoil.Count +
                        0.1f * listCoil.Count +
                        0.2f * totalDistance / listCoil.Count;

            var scaledDifficulty = Mathf.Clamp01((totalDifficulty - 0) / (10 - 0));
            var finalScore = (1 + scaledDifficulty * 9) / 2f;

            levelBase.Stages[i].RandomCoils = string.Join(",", listCoil.OrderBy(o => o.color).Select(x => x.color).ToArray());
            s = string.Format("{0}\n - stage {1}: {2} : {3} ->\n{4}", s, i, group.Count(), totalDifficulty, string.Join("\n", group.Select(x => string.Format("      {0}: {1}", x.Key, x.Count()))));
        }


        analyticLevel = string.Format("Count Stage: {0}\n {1} ", levelBase.Stages.Length, s);
    }
}
