using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasTestController : Kit.Common.Singleton<CanvasTestController>
{
    [SerializeField] GameObject m_WinPanel;
    [SerializeField] GameObject m_LostPanel;
    [SerializeField] TMPro.TMP_Text m_LevelText;

    public TMPro.TMP_Text levelText => m_LevelText;

    public void ShowWinPanel()
    {
        m_WinPanel.gameObject.SetActive(true);
    }

    public void ShowLostPanel()
    {
        m_LostPanel.gameObject.SetActive(true);

    }

    public void ButtonReLoadTap()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ButtonNextLevelTap()
    {
        m_WinPanel.gameObject.SetActive(false);
        ButtonReLoadTap();
    }

    public void ButtonRetryLevelTap()
    {
        m_LostPanel.gameObject.SetActive(false);
        ButtonReLoadTap();
    }
}
