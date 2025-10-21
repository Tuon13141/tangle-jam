using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextRemoteConfig : MonoBehaviour
{
    public RemoteConfigKey key;
    public bool useString;
    public string stringKey;
    private TMP_Text Text1;
    private Text Text2;

    void Start()
    {
        Text1 = GetComponent<TMP_Text>();
        Text2 = GetComponent<Text>();
    }

    public void Set(string me)
    {
        if (Text1 != null)
            Text1.text = me;
        if (Text2 != null)
            Text2.text = me;
    }

    private void Update()
    {
        if (Time.time > 3)
        {
            if (!useString)
            {
                Set(key + "= " + key.GetValueDouble().ToString());
            }
            else
            {
                Set(key + "= " + stringKey + " " + RemoteConfigHelper.GetValueDouble(stringKey));
            }
        }
    }
}