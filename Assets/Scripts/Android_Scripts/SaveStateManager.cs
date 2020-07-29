using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SaveStateManager : DefaultTrackableEventHandler
{
    private Manager _m;
    
    private void Awake()
    {
        _m = GameObject.Find("LogicManager").GetComponent<Manager>();
    }

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
        StartCoroutine(GetRequest(_m.GetUrlPath() + "/save_state"));
    }
    
    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError) print("Error: cannot retrieve information from server");
            else _m.SaveState(webRequest.downloadHandler.text);
        }
    }
}
