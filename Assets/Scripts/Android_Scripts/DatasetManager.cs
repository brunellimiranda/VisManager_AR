using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Vuforia;
public class DatasetManager : DefaultTrackableEventHandler
{
    private Manager _m;
    private RadialMenuBehavior _rm;
    private bool _flagSwitch;
    private bool _extFlag;
    
    // Start is called before the first frame update
    void Awake()
    {
        _m = GameObject.Find("LogicManager").GetComponent<Manager>();
        _rm = GetComponentInChildren<RadialMenuBehavior>();
        
    }
    
    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
 
        _rm.gameObject.SetActive(true);
        _flagSwitch = true;
        LoadGrid();
    }

    private void LoadGrid()
    {

        string url_Server = _m.GetUrlPath();
        string requestDatasets = url_Server + "/info.html";
        print("datasets path: " + requestDatasets);

        StartCoroutine(GetRequest(requestDatasets));
    }
    
    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                print("Error: cannot retrieve information from server");
            }
            else
            {
                GetWWWText(webRequest.downloadHandler.text);
            }
        }
    }
    
    private void GetWWWText(string base64str)
    {
        string[] datasets = base64str.Split(',');
        List<string> datasetOptions = new List<string>(datasets.ToList());

        _rm.SetRadialOptions(datasetOptions);
        
    }
    
    public void NextPage()
    {
        _rm.GetComponent<RadialMenuBehavior>().UpdatePage(1);
    }
}
