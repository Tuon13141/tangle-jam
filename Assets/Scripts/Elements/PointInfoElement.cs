using System;
using UnityEngine;
using TMPro;

public class PointInfoElement : MonoBehaviour
{
    [SerializeField] TMP_Text m_Text;
    [SerializeField] GameObject m_iconNoti;

    RectTransform rectTransform;
    private void Awake()
    {
        isShow = gameObject.activeSelf;
    }

    Vector3 target;
    private void LateUpdate()
    {
        if (!isShow) return;
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        rectTransform.SetPos(target, CollectionController.instance.maincamera);
    }

    public void SetIndex(int value)
    {
        Show();
        m_Text.text = string.Format("{0}", value);

        if (value == 0)
        {
            //fill done object
            Hide();
        }
    }

    public void SetPosition(Vector3 target)
    {
        this.target = target;
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        Debug.Log($"SetPosition : {target}");
        rectTransform.SetPos(target, CollectionController.instance.maincamera);
    }

    bool isShow;
    void Show()
    {
        if (isShow) return;
        isShow = true;

        //dotween
        gameObject.SetActive(true);

    }

    void Hide()
    {
        if (!isShow) return;
        isShow = false;

        //dotween
        gameObject.SetActive(false);

    }
}
