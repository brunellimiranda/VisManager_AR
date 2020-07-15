using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Vuforia;
using TMPro;

/// <summary>
/// Script for requesting datasets from server and showing their names onscreen
/// Used with ReticleSelectionBehaviour
/// Server Path stored in Manager
/// Found at Load Dataset ImageTarget
/// </summary>
/// 
public class ServerDatasetReader_V2 : DefaultTrackableEventHandler
{
    public GameObject radialGrid;
    public Transform cameraTransform;
    private bool flagSwitch;
    private bool extFlag;
    public GameObject reticle;

    private float timer = 0.0f;
    private float waitTime = 3.0f;

    private void Awake()
    {
        IEnumerable<TrackableBehaviour> tbs = TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();
    }

    //private void Update()
    //{
    //    if (extFlag)
    //    {
    //        print("timer: " + timer);
    //        timer += Time.deltaTime;
    //        if (timer > waitTime)
    //        {
    //            flagSwitch = false;
    //            grid.SetActive(false);
    //            reticle.SetActive(false);

    //            timer = 0;
    //            extFlag = false;
    //        }
    //    }
    //}

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
        //GameObject.Find("LogicManager").GetComponent<Manager>().SetActiveImageTarget(this.gameObject);
        //print("track: " + this.gameObject.GetComponent<TrackableBehaviour>().CurrentStatus);
        if (!isActive(radialGrid))
        {
            radialGrid.SetActive(true);
            flagSwitch = true;
            LoadGrid(radialGrid);
        }
        //if((isActive(grid)) && (this.gameObject.GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.EXTENDED_TRACKED))
        //{
        //    extFlag = true;
        //}
    }

    //protected override 
    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();
        if (isActive(radialGrid))
        {
            flagSwitch = false;
            radialGrid.SetActive(false);
            reticle.SetActive(false);
        }
    }

    private bool isActive(GameObject subject)
    {
        return subject.gameObject.activeInHierarchy;
    }

    private void LoadGrid(GameObject gridObj)
    {
        GameObject theManager = GameObject.Find("LogicManager");
        Manager managerLocal = theManager.GetComponent<Manager>();

        string url_Server = managerLocal.GetUrlPath();
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
        string[] datasets;
        datasets = base64str.ToString().Split(","[0]);

        //Load Grid panels based on the number of datasets in the server
        LoadGridPanels(datasets);
    }


    private void LoadGridPanels(string[] datasetName)
    {
        int quantity = datasetName.Length;
        int qCh = radialGrid.transform.childCount;
        print("children: " + qCh);

        if (quantity <= qCh)
        {
            for (int i = 0; i < qCh; i++)
            {
                if (i < quantity)
                {
                    var childObj = radialGrid.transform.Find("fatia" + i);
                    childObj.gameObject.SetActive(true);
                    childObj.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = datasetName[i];
                }
                else
                {
                    radialGrid.transform.Find("fatia" + i).gameObject.SetActive(false);
                }
            }
        }
        //print("alcancou a chamada");
        turnOnReticle();
    }

    private void turnOnReticle()
    {
        //GameObject theManager = GameObject.Find("LogicManager");
        //Manager managerLocal = theManager.GetComponent<Manager>();
        //managerLocal.SetReticleStatus(true);
        reticle.SetActive(true);
    }
}
