using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetPlayerPref : MonoBehaviour
{
    public string playerPrefKey = "test_device_id";
    public InputField input;
    public Button button;

    public void Start()
    {
        input.text = PlayerPrefs.GetString(playerPrefKey);
        button.onClick.AddListener(() =>
        {
            PlayerPrefs.SetString(playerPrefKey,input.text);
        });
    }
}
