using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Percas.Data;
using Percas.UI;
using UnityEngine.EventSystems;

public class ButtonFill : ButtonBase
{
    [SerializeField] TMP_Text m_TextButton;
    [SerializeField] TMP_Text m_TextInfo;

    public bool isHoldToBuild = false;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (isHoldToBuild)
        {
            CollectionController.instance.Fill();
            //RoomController.OnDisplayButtonClose?.Invoke(false);
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (isHoldToBuild)
        {
            CollectionController.instance.StopFill();
            //RoomController.OnDisplayButtonClose?.Invoke(true);
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (!isHoldToBuild)
        {
            CollectionController.instance.Fill();
            RoomController.OnDisplayButtonClose?.Invoke(false);
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public async void ResetButtonText(int value, LogCurrency log)
    {
        await UniTask.DelayFrame(1);
        SetIndexFill(PlayerDataManager.PlayerData.Coil);
    }

    public void SetIndexFill(int value)
    {
        m_TextButton.text = string.Format("Build <sprite=0> {0}", value);
    }
}
