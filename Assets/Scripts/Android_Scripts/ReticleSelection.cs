﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using TMPro;
using UnityEngine.Networking;
using System.Globalization;
using UnityEngine.Animations;

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

    private Manager m;
    private ProjectUtils utils;

    private void Awake()
    {
        center = new Vector2((Screen.width / 2), (Screen.height / 2));
        m = GameObject.Find("LogicManager").GetComponent<Manager>();
        utils = GameObject.Find("LogicManager").GetComponent<ProjectUtils>();
    }

    private void Update()
    {
        //print("Choosed Dataset: " + GameObject.Find("LogicManager").GetComponent<Manager>().GetDatasetLoaded());
        Ray ray = Camera.main.ScreenPointToRay(center);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit)) return;

        string request1;
        switch (hit.transform.parent.tag)
        {
            case "LoadDataset_Item":
                newDatasetName = hit.transform.GetComponentInChildren<UnityEngine.UI.Text>().text;
                if (previousDatasetName != null)
                {
                    if (previousDatasetName == newDatasetName)
                    {
                        timer += Time.deltaTime;
                        if (!(timer > dwellTime)) return;
                        print("acertou base: " + hit.transform.GetComponentInChildren<UnityEngine.UI.Text>().text);
                        m.SetDatasetLoaded(newDatasetName);
                        
                        
                        timer = 0;
                        requestDataset = m.GetDatasetLoaded();
                        requestServerPath = m.GetUrlPath();
                        request1 = requestServerPath + "/attributes/" + requestDataset;
                        print("Resquest 1: " + request1);
                        StartCoroutine(GetRequest(request1, 0));
                        string request2 = requestServerPath + "/row/" + requestDataset + "/0";
                        print("Resquest 2: " + request2);
                        StartCoroutine(GetRequest(request2, 1));
                        return;
                    }
                }
                previousDatasetName = newDatasetName;
                break;
                
            case "LoadAttribute_Item":
                timer += Time.deltaTime;
                if (!(timer > dwellTime)) return;
                timer = 0;

                print("acertou atributo: " + hit.transform.GetComponentInChildren<UnityEngine.UI.Text>().text);

                if (hit.transform.GetComponentInChildren<UnityEngine.UI.Text>().text == "next page")
                {
                    GameObject.Find("Filter_Target").GetComponent<AttributeSelector>().NextPage();
                    return;
                }
                
                requestDataset = m.GetDatasetLoaded();
                requestServerPath = m.GetUrlPath();
                string requestAttribute = hit.transform.GetComponentInChildren<UnityEngine.UI.Text>().text;
                request1 = requestServerPath + "/field/" + requestDataset + "/" + requestAttribute;

                m.SetLastSelected(requestAttribute);
                GameObject.Find("SetX_Target").GetComponent<CategoricSelector>().SetNewAttribute(requestAttribute);
                
                print("Resquest 3: " + request1);
                StartCoroutine(GetRequest(request1, 2));
                break;
            
            case "LoadCategoricOptions_Item":
                timer += Time.deltaTime;
                if (!(timer > dwellTime)) return;
                timer = 0;

                
                print("selecionou categoria: " + hit.transform.GetComponentInChildren<UnityEngine.UI.Text>().text);

                
                if (hit.transform.GetComponentInChildren<UnityEngine.UI.Text>().text == "next page")
                {
                    GameObject.Find("SetX_Target").GetComponent<CategoricSelector>().NextPage();
                }
                
                break;
            
            default:
                print("card not implemented yet");
                break;
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
                switch (level)
                {
                    case 0:
                        GetWWWAttributes(webRequest.downloadHandler.text);
                        break;
                    
                    case 1: 
                        GetWWWFirstLine(webRequest.downloadHandler.text);
                        break;
                    
                    case 2:
                        GetWWWRow(webRequest.downloadHandler.text);
                        break;
                        
                    default:
                        print("Error: the requisition " + level + " has not implemented yet");
                        break;
                }
            }
        }
    }

    private void GetWWWRow(string base64)
    {
        print(base64);
        m.SetCategories(base64.Split(','));
        GameObject.Find("SetX_Target").GetComponent<CategoricSelector>().RefreshAttribute();
    }

    private void GetWWWAttributes(string base64str)
    {
        attributes = base64str.Split(","[0]);
        m.SetAttributes(attributes);
        GameObject.Find("Filter_Target").GetComponent<AttributeSelector>().UpdateGrid();

    }

    private void GetWWWFirstLine(string base64str)
    {
        firstRowData = base64str.Split(',');
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

        m.SetTypeOfAttribute(typeOfAttributes);
        m.SetDatasetStatus(true);
    }
}
