using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RewardInfo : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text m_Text;

    float minDistance;

    private void Awake()
    {
        float minDimension = Mathf.Min(Screen.width, Screen.height);
        minDistance = 0.05f * minDimension;
    }

    GameObject currentSelectedGameObject = null;
    Vector3 mousePositionDown;
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            currentSelectedGameObject = null;
            if (Vector3.Distance(mousePositionDown, Input.mousePosition) > minDistance)
            {
                Hide();
            }
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);

        mousePositionDown = Input.mousePosition;
        currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
    }

    public void SetText(int count)
    {
        m_Text.text = string.Format("<sprite=0> {0}", count);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
