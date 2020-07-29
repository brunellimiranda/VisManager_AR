using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoadStateManager : DefaultTrackableEventHandler
{
    private Manager _m;
    // Start is called before the first frame update
    void Awake()
    {
        _m = GameObject.Find("LogicManager").GetComponent<Manager>();
        
    }

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
        string state = _m.LoadState();

        if (state == "state saved")
        {
            StartCoroutine(GetRequest(_m.GetUrlPath() + "/load_state"));
            return;
        }
        Debug.LogError("Please, save one state before!");
    }
    

   
    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError) print("Error: cannot retrieve information from server");
            else GetComponentInChildren<AR_ChartGenerator>().GetChart(webRequest.downloadHandler.text);

        }
    } 
}

