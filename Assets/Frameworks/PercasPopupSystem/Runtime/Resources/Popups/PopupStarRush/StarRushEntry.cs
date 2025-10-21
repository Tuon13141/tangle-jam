using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Percas.Data;
using Percas;

public class StarRushEntry : MonoBehaviour
{
    [SerializeField] List<Sprite> backgroundSprites;
    [SerializeField] List<Color> colors;
    [SerializeField] List<GameObject> positions;
    [SerializeField] Image m_background;
    [SerializeField] Image m_image, m_frameBG, m_frame;
    [SerializeField] TMP_Text m_textPlayerPosition, m_textPlayerName, m_textPlayerScore;
    [SerializeField] GameObject m_giftBox;

    public void Init(string playerProfile, int score, int position, bool isYou)
    {
        m_background.sprite = isYou ? backgroundSprites[1] : backgroundSprites[0];
        m_textPlayerName.color = position >= 1 ? colors[1] : colors[0];
        m_giftBox.SetActive(position == 0);

        string playerName = PlayerDataManager.PlayerData.GetPlayerName(playerProfile);
        int avatarID = PlayerDataManager.PlayerData.GetAvatarID(playerProfile);
        int frameID = PlayerDataManager.PlayerData.GetFrameID(playerProfile);
        m_textPlayerName.text = isYou ? $"You" : $"{playerName}";

        positions.ForEach((pos) => pos.SetActive(false));
        int positionIndex = Mathf.Clamp(position, 0, 3);
        positions[positionIndex].SetActive(true);

        m_image.sprite = DataManager.Instance.GetAvatarImage(avatarID);
        m_frameBG.sprite = DataManager.Instance.GetAvatarFrameBG(frameID);
        m_frame.sprite = DataManager.Instance.GetAvatarFrame(frameID);
        m_textPlayerPosition.text = $"{position + 1}";
        m_textPlayerScore.text = $"{score}";
    }
}
