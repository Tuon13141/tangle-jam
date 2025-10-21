using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementCreate : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] GridElement m_GridElement;
    [SerializeField] PrefabManager m_PrefabManager;

    [HorizontalLine(2, EColor.Blue)]
    public StageData.CellType type;
    public int value;
    public StageData.Direction direction;

    public GridElement gridElement
    {
        get
        {
            if (m_GridElement == null)
            {
                m_GridElement = GetComponent<GridElement>();
            }

            return m_GridElement;
        }
    }

    [ContextMenu("Setup")]
    public void Setup()
    {
        switch (type)
        {
            case StageData.CellType.Empty:
                EmptyElement();
                break;

            case StageData.CellType.Wall:
                WallElement();
                break;

            case StageData.CellType.Coil:
                CoilElement();
                break;

            case StageData.CellType.Stack:
                StackCoilElement();
                break;

            case StageData.CellType.CoilLocked:
                CoilMyteryElement();
                break;

            case StageData.CellType.PinWall:
                PinWallElement();
                break;

            case StageData.CellType.PinControl:
                PinControlElement();
                break;
        }
    }

    //[Button]
    public void EmptyElement()
    {
        type = StageData.CellType.Empty;
        gridElement.Setup(StageData.CellType.Empty, gridElement.controller);
        gridElement.holder.ClearContent();
    }

    //[Button]
    public void WallElement()
    {
        type = StageData.CellType.Wall;
        gridElement.Setup(StageData.CellType.Wall, gridElement.controller);
        gridElement.holder.ClearContent();
    }

    //[Button]
    public void CoilElement()
    {
        type = StageData.CellType.Coil;
        gridElement.Setup(StageData.CellType.Coil, gridElement.controller);
        gridElement.holder.ClearContent();
        var color = FindObjectOfType<LevelCreate>().pictureAsset.Colors[value];
        var coilElement = Static.InstantiateUtility(m_PrefabManager.GetCoilPrefab(), gridElement.holder);

        coilElement.ChangeColor(color);
    }

    //[Button]
    public void CoilMyteryElement()
    {
        type = StageData.CellType.CoilLocked;
        gridElement.Setup(StageData.CellType.Coil, gridElement.controller);
        gridElement.holder.ClearContent();
        var color = FindObjectOfType<LevelCreate>().pictureAsset.Colors[value];
        var coilElement = Static.InstantiateUtility(m_PrefabManager.GetCoilPrefab(), gridElement.holder);

        coilElement.ChangeColor(color);
        coilElement.ChangeStatus(CoilStatus.Mystery);
    }


    //[Button]
    public void StackCoilElement()
    {
        type = StageData.CellType.Stack;
        gridElement.Setup(type, gridElement.controller);
        gridElement.holder.ClearContent();

        var tubeElement = Static.InstantiateUtility(m_PrefabManager.GetTubePrefab(), gridElement.holder);
        tubeElement.SetupDirection(direction);
        tubeElement.countText.text = string.Format("{0}", value);
    }

    //[Button]
    public void PinControlElement()
    {
        type = StageData.CellType.PinControl;
        gridElement.Setup(type, gridElement.controller);
        gridElement.holder.ClearContent();

        var pinControl = Static.InstantiateUtility(m_PrefabManager.GetPinControlPrefab(), gridElement.holder);
        pinControl.SetupDirection(direction);
    }

    //[Button]
    public void PinWallElement()
    {
        type = StageData.CellType.PinWall;
        gridElement.Setup(type, gridElement.controller);
        gridElement.holder.ClearContent();

        var pinWall = Static.InstantiateUtility(m_PrefabManager.GetPinWallPrefab(), gridElement.holder);
        pinWall.SetupDirection(direction);
    }

    //[Button]
    public void CoilDouble()
    {
        type = StageData.CellType.CoilPair;
        gridElement.Setup(type, gridElement.controller);
        gridElement.holder.ClearContent();

        var color = FindObjectOfType<LevelCreate>().pictureAsset.Colors[value];
        var coilElement = Static.InstantiateUtility(m_PrefabManager.GetCoilPrefab(), gridElement.holder);

        coilElement.ChangeColor(color);
        coilElement.SetFakeConnect(true, direction);
    }

    public void ButtonStack()
    {
        Debug.Log("ButtonStack");
        type = StageData.CellType.ButtonStack;
        gridElement.Setup(type, gridElement.controller);
        gridElement.holder.ClearContent();

        var buttons = Static.InstantiateUtility(m_PrefabManager.GetButtonsElementPrefab(), gridElement.holder);
        buttons.countText.text = string.Format("{0}", value);
        //coilElement.ChangeColor(color);
        //coilElement.SetFakeConnect(true, direction);
    }
#endif
}
