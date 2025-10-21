using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Sonat;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

public class DownloadLevelManager : BaseService
{
    public delegate T DefaultLoadDataRaw<T>(string path);
    private LevelRemoteConfigData levelRemoteConfigData;
    private AssetBundle assetBundle;
    private static PlayerPrefString lastId;
    private bool outOfSegment;
    private PlayerPrefString cachedRemoteValue;
    public static string mapId;
    private static JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

	private void Awake()
	{
        lastId = new PlayerPrefString("LAST_REMOTE_LEVEL_DATA_ID", "");
        cachedRemoteValue = new PlayerPrefString("CACHED_REMOTE_VALUE", "");
    }


	private void Start()
	{
        StartCoroutine(WaitLoadRemoteConfig());
	}

    IEnumerator WaitLoadRemoteConfig()
	{
        float t = 0;
        float timeWait = 3f;
        while (!FireBaseController.FireBaseRemoteReady && t < timeWait)
        {
            t += Time.deltaTime;
            yield return null;
        }
        GetRemoteData();
	}

	private void GetRemoteData(int currentLevel = -1)
	{
        string json = RemoteConfigKey.remote_level_data.GetValueString();
        
        if(string.IsNullOrEmpty(json) && !Kernel.IsInternetConnection())
		{
            json = cachedRemoteValue.Value;
		}
		if (!string.IsNullOrEmpty(json))
		{
			if (!cachedRemoteValue.Value.Equals(json))
			{
                cachedRemoteValue.Value = json;
				if (PlayerPrefs.HasKey("DOWN_LOAD_LEVEL_LOG"))
				{
                    PlayerPrefs.DeleteKey("DOWN_LOAD_LEVEL_LOG");
				}
			}
            levelRemoteConfigData = JsonConvert.DeserializeObject<LevelRemoteConfigData>(json);
            levelRemoteConfigData.ParseData();
            if(string.IsNullOrEmpty(lastId.Value) || levelRemoteConfigData.id != lastId.Value)
			{
                if (currentLevel < 0) currentLevel = DummyLogParameterValueRetriever.UserPropertyLevel;
                if (currentLevel > levelRemoteConfigData.levelStart)
				{
                    outOfSegment = true;
                    Initialized = true;
                }
				else
				{
                    //UnityWebRequestAssetBundle.
                    //lastVersion.Value++;
				}
			}
		}
		else
		{
            outOfSegment = true;
            Initialized = true;
		}

		if (!outOfSegment)
		{
            StartCoroutine(DownloadAssetBundle());
        }
	}

    private IEnumerator DownloadAssetBundle()
	{
        Debug.Log("DownloadLevelManager: start load remote level data");
        Hash128 hash128 = new Hash128();
        hash128.Append(levelRemoteConfigData.id);
        CachedAssetBundle cached = new CachedAssetBundle(levelRemoteConfigData.id, hash128);
        LogDownLoadPhase("start", levelRemoteConfigData.id);
        using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(levelRemoteConfigData.url, cached, 0))
		{
            yield return www.SendWebRequest();
            if(www.result != UnityWebRequest.Result.Success)
			{
                Debug.Log("DownloadLevelManager: request fail!!");
                LogDownLoadPhase("failed", levelRemoteConfigData.id);
                Initialized = true;
            }
			else
			{
                Debug.Log("DownloadLevelManager: download success!");
                assetBundle = DownloadHandlerAssetBundle.GetContent(www);
                if (assetBundle != null)
                    lastId.Value = levelRemoteConfigData.id;
                yield return new WaitForSeconds(0.1f);
                Initialized = true;
                LogDownLoadPhase("success", levelRemoteConfigData.id);
                PlayerPrefs.SetInt("DOWN_LOAD_LEVEL_LOG", 1);
            }
            
		}
	}

    public T GetData<T>(string fileName, DefaultLoadDataRaw<T> defaultLoad)
	{
		if (!outOfSegment && assetBundle != null)
		{
			TextAsset textAsset = assetBundle.LoadAsset<TextAsset>(fileName);
			if (textAsset != null && !string.IsNullOrEmpty(textAsset.text))
			{
                var data = JsonConvert.DeserializeObject<T>(textAsset.text, settings);
                if (data != null)
                {
                    mapId = lastId.Value;
                }
                else mapId = "";
                return data;
			}
		}
        Debug.Log("DownloadLevelManager: default!!");
        mapId = "";
        if (defaultLoad != null) return defaultLoad.Invoke(fileName);
        return default;
	}

    private void LogDownLoadPhase(string phase, string map_id)
	{
        if (PlayerPrefs.HasKey("DOWN_LOAD_LEVEL_LOG")) return;
#if !dummy_log
        new SonatLogDownloadLevelData()
        {
            phase = phase,
            map_id = map_id
        }.Post();
#endif
    }

}

public class LevelRemoteConfigData
{
    public string url;
    public string id;
   // public uint version;
    public int order;
    public int levelStart;
    public int levelEnd;

    public void ParseData()
	{
        var temp = url.Split('/');
        id = temp[temp.Length - 1];
        //string pattern = @"a_d(?<date>.+)f(?<from>.+)t(?<to>.+)";
        //RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase;

        //Match match = Regex.Match(id, pattern, options);
        //if (match.Success)
        //{
        //    levelStart = int.Parse(match.Groups["from"].Value);
        //    levelEnd = int.Parse(match.Groups["to"].Value);
        //}

        string[] data = id.Split('_');
        if (data[data.Length - 1].ToLower().Equals("alllevel"))
        {
            levelStart = 1;
            levelEnd = 999999;
        }
        else
        {
            levelStart = int.Parse(data[data.Length - 2]);
            levelEnd = int.Parse(data[data.Length - 1]);
        }
    }
}
