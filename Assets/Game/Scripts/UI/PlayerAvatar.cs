using System;
using UnityEngine;
using UnityEngine.UI;
using Percas;
using Percas.Data;

public class PlayerAvatar : MonoBehaviour, IActivatable
{
    [SerializeField] bool realTimeUpdate = true;
    [SerializeField] Image m_image, m_frameBG, m_frame;

    public static Action<bool, int, int> OnUpdateUI;

    private void Awake()
    {
        OnUpdateUI += UpdateUI;
    }

    private void OnDestroy()
    {
        OnUpdateUI -= UpdateUI;
    }

    private void OnEnable()
    {
        UpdatePlayerAvatar(PlayerDataManager.PlayerData.GetAvatarID(), PlayerDataManager.PlayerData.GetFrameID());
    }

    public void Activate()
    {
        UpdatePlayerAvatar(PlayerDataManager.PlayerData.GetAvatarID(), PlayerDataManager.PlayerData.GetFrameID());
    }

    public void Deactivate() { }

    private void UpdatePlayerAvatar(int image, int frame)
    {
        m_image.sprite = DataManager.Instance.GetAvatarImage(image);
        m_frameBG.sprite = DataManager.Instance.GetAvatarFrameBG(frame);
        m_frame.sprite = DataManager.Instance.GetAvatarFrame(frame);
    }

    private void UpdateUI(bool forceToUpdate, int image, int frame)
    {
        if (!forceToUpdate && !realTimeUpdate) return;
        if (image >= 0) m_image.sprite = DataManager.Instance.GetAvatarImage(image);
        if (frame >= 0) m_frameBG.sprite = DataManager.Instance.GetAvatarFrameBG(frame);
        if (frame >= 0) m_frame.sprite = DataManager.Instance.GetAvatarFrame(frame);
    }
}
