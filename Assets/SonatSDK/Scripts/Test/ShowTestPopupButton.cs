using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowTestPopupButton : MonoBehaviour
{
    [SerializeField]
    private TestPopup testPopup;

    public TestPopup TestPopup
    {
        get
        {
            if (testPopup == null)
                testPopup = FindObjectOfType<TestPopup>(true);
            return testPopup;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            TestPopup.gameObject.SetActive(!TestPopup.gameObject.activeSelf);
        });
    }

 
}
