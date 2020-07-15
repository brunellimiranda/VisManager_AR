using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using TMPro;
using UnityEngine.Networking;
using System.Globalization;

/// <summary>
/// Script for selection of a specific dataset from server
/// Played after ServerDatasetReader
/// Selected dataset name is stored in Manager scrip for future references
/// Attributes are categorized here *Maybe change it later to another script
/// Found inside ARCamera-->Reticle
/// </summary>

public class ReticleSelection : MonoBehaviour
{
    public Vector2 center;

    private string previousDatasetName;
    private string newDatasetName;

    private float timer = 0.0f;
    private float dwellTime = 1.5f;

    private string requestDataset;
    private string requestServerPath;

    private string[] attributes;
    private string[] typeOfAttributes;
    private string[] firstRowData;

    private Manager localManager;

    private void Awake()
    {
        center = new Vector2((Screen.width / 2), (Screen.height / 2));
        localManager = GameObject.Find("LogicManager").GetComponent<Manager>();
    }

    private void Update()
    {
        //print("Choosed Dataset: " + GameObject.Find("LogicManager").GetComponent<Manager>().GetDatasetLoaded());
        Ray ray = Camera.main.ScreenPointToRay(center);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.parent.tag == "LoadDataset_Item")
            {
                newDatasetName = hit.transform.GetChild(0).transform.GetChild(0).transform.GetComponent<UnityEngine.UI.Text>().text.ToString();
                if (previousDatasetName != null)
                {
                    if (previousDatasetName == newDatasetName)
                    {
                        timer += Time.deltaTime;
                        if (timer > dwellTime)
                        {
                            print("acertou: " + hit.transform.GetChild(0).transform.GetChild(0).transform.GetComponent<UnityEngine.UI.Text>().text);
                            localManager.SetDatasetLoaded(newDatasetName);
                            timer = 0;
                            requestDataset = localManager.GetDatasetLoaded();
                            requestServerPath = localManager.GetUrlPath();
                            string request1 = requestServerPath + "/attributes/" + requestDataset;
                            print("Resquest 1: " + request1);
                            StartCoroutine(GetRequest(request1, 0));
                            string request2 = requestServerPath + "/row/" + requestDataset + "/0";
                            print("Resquest 2: " + request2);
                            StartCoroutine(GetRequest(request2, 1));
                        }
                    }
                }
            }
            previousDatasetName = newDatasetName;
        }
    }

    IEnumerator GetRequest(string uri, int level)
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
                if (level == 0)
                {
                    GetWWWAttributes(webRequest.downloadHandler.text);
                }
                else
                {
                    GetWWWFirstLine(webRequest.downloadHandler.text);
                }

            }
        }
    }

    private void GetWWWAttributes(string base64str)
    {
        attributes = base64str.ToString().Split(","[0]);
        localManager.SetAttributes(attributes);

    }

    private void GetWWWFirstLine(string base64str)
    {
        firstRowData = base64str.ToString().Split(","[0]);
        typeOfAttributes = new string[firstRowData.Length];

        int i = 0;

        foreach (string x in firstRowData)
        {
            if (float.TryParse(x, NumberStyles.Any, CultureInfo.InvariantCulture, out float temp))
            {
                typeOfAttributes[i] = "CONT";
            }
            else
            {
                typeOfAttributes[i] = "CAT";
            }
            i++;
        }

        localManager.SetTypeOfAttribute(typeOfAttributes);
        localManager.SetDatasetStatus(true);
    }
}
