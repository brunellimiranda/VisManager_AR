﻿using System.Collections;
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

public class ReticleSelectionBehaviour : MonoBehaviour
{
    public Vector2 center;

    private string previousDatasetName;
    private string newDatasetName;

    private float timer = 0.0f;
    private float dwellTime = 3.0f;

    private string requestDataset;
    private string requestServerPath;

    private string[] attributes;
    private string[] typeOfAttributes;
    private string[] firstRowData;

    private void Awake()
    {
        center = new Vector2((Screen.width / 2), (Screen.height/2));
    }

    private void Update()
    {
        //print("Choosed Dataset: " + GameObject.Find("LogicManager").GetComponent<Manager>().GetDatasetLoaded());
        Ray ray = Camera.main.ScreenPointToRay(center);
        RaycastHit hit;
        
        if(Physics.Raycast(ray, out hit))
        {
            newDatasetName = hit.transform.GetChild(0).transform.GetComponent<TextMeshPro>().text;
            if(previousDatasetName != null)
            {
                if(previousDatasetName == newDatasetName)
                {
                    timer += Time.deltaTime;
                    if(timer > dwellTime)
                    {
                        GameObject.Find("LogicManager").GetComponent<Manager>().SetDatasetLoaded(newDatasetName);
                        timer = 0;
                        requestDataset = GameObject.Find("LogicManager").GetComponent<Manager>().GetDatasetLoaded();
                        requestServerPath = GameObject.Find("LogicManager").GetComponent<Manager>().GetUrlPath();
                        string request1 = requestServerPath + "/attributes/" + requestDataset;
                        print("Resquest 1: " + request1);
                        StartCoroutine(GetRequest(request1, 0));
                        string request2 = requestServerPath + "/row/" + requestDataset + "/0";
                        StartCoroutine(GetRequest(request2, 1));
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
                if(level == 0)
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

        //for (int i = 0; i < attributes.Length; i++)
        //{
        //    print("attr[" + i + "]: " + attributes[i]);
        //}
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
            print("Att[" + i + "]_type: " + typeOfAttributes[i]);
            i++;
        }
    }
}