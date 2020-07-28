using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using TMPro;
using UnityEngine.Networking;
using System.Globalization;
using UnityEngine.Animations;
using UnityEngine.UI;

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

    private string _datasetName;

    private float _timer = 0.0f;
    private float dwellTime = 1.5f;

    private string _requestDataset;
    private string _requestServerPath;

    private string[] _attributes;
    private string[] _typeOfAttributes;
    private string[] _firstRowData;
    bool pl;

    public Material[] mColors;

    private Manager _m;
    private ProjectUtils _u;

    private void Awake()
    {
        center = new Vector2((Screen.width / 2), (Screen.height / 2));
        _m = GameObject.Find("LogicManager").GetComponent<Manager>();
        _u = GameObject.Find("LogicManager").GetComponent<ProjectUtils>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            GameObject.Find("Filter_Target").GetComponent<AR_FilterManager>().GenerateFilter();
        }

        Ray ray = Camera.main.ScreenPointToRay(center);
        RaycastHit hit;
        
        
        if (!Physics.Raycast(ray, out hit)) return;
        if (hit.transform.CompareTag("blocker")) return;
        
        Transform go = hit.transform;
        
        
        if (hit.transform.CompareTag("SliderButton") || hit.transform.parent.CompareTag("SliderButton"))
        {
            go.GetComponentInParent<SliderManager>().UpdateValues(go.transform);

            return;
        }
        
        if (hit.transform.CompareTag("Numeric_ITem") || hit.transform.parent.CompareTag("Numeric_ITem"))
        {
            go.GetComponentInParent<SelectorManager>().UpdateFilterValues();
            return;
        }
        
        ChangeMenuCollors(hit);
        _timer += Time.deltaTime;
        if (!(_timer > dwellTime)) return;
        _timer = 0;

        if (hit.transform.CompareTag("check"))
        {
            print("Seleção invertida aplicada!");
            go.GetComponentInParent<SelectorManager>().SetInvertedSelection();
            return;
        }
        
        go.GetComponent<MeshRenderer>().material = mColors[1];

        int visId;
        string option = go.GetComponentInChildren<Text>().text;

        switch (hit.transform.parent.tag)
        {
            case "LoadDataset_Item":
                _datasetName = option;
                
                if (_datasetName != null)
                {
                    //Action
                    print("selecionou database " + option);
                    
                    _m.SetDatasetLoaded(option);
                    _requestServerPath = _m.GetUrlPath();
                    string request1 = _requestServerPath + "/attributes/" + option;
                    
                    print("Request 1: " + request1);
                    StartCoroutine(GetRequest(request1, 0));
                    return;
                }
                break;
            
            // ==== Vis Card ==== //
            case "VisOption_Item":
                print("acertou vis: " + option);
                int parentId = go.GetComponentInParent<ChartManager>().GetId();
                go.GetComponentInParent<ChartManager>().SetVisType(option);
                _m.AddNewActiveVisualization(parentId, option);
                break;
            
            // ==== Axis Cards ==== //
            case "AxisVisOption_Item":
                print("Selecionou Vis Ativa: " + option);
                pl = option.Contains("parallel_coordinates");
                go.GetComponentInParent<AxisManager>().SelectedVis(option);
                break;
            
            case "AxisOption_Item":
                if (go.GetComponentInChildren<Text>().text == "more options")
                {
                    GameObject.Find("SetX_Target").GetComponent<AxisManager>().NextPage();
                    return;
                }
                
                print("Selecionou Opção: " + option);
                go.GetComponentInParent<AxisManager>().SelectedAxis(option, pl);
                break;
            
            case "AxisAttribute_Item":
                if (go.GetComponentInChildren<Text>().text == "more options")
                {
                    GameObject.Find("SetX_Target").GetComponent<AxisManager>().NextPage();
                    return;
                }
                
                print("Selecionou Atributo: " + option);
                go.GetComponentInParent<AxisManager>().SetAxisOnVis(option);
                break;
            
            // ==== Clear Card ==== //
            case "ClearOption_Item":
                go.GetComponentInParent<ClearCardManager>().ClearOption(option);
                break;
            
            case "ClearVis_Item":
                visId = Int32.Parse(option[0].ToString());
                go.GetComponentInParent<ClearCardManager>().ClearVis(visId);
                break;
            
            case "ClearAxis_Item":
                visId = Int32.Parse(option[0].ToString());
                go.GetComponentInParent<ClearCardManager>().ClearAxis(visId);
                break;
            
            // ==== Filter Card ==== //
            case "Attribute_Item":
                AR_FilterManager FilterTarget = GameObject.Find("Filter_Target").GetComponent<AR_FilterManager>();
                if (option == "more options")
                {
                    FilterTarget.NextPage(1);
                    return;
                }
                
                print("acertou atributo: " + option);
                FilterTarget.UpdateFilter(option);
                break;
            
            case "Categoric_Item":
                if (option == "more options")
                { 
                    go.GetComponentInParent<SelectorManager>().NextPage(1);
                    return;
                }

                go.GetComponentInParent<SelectorManager>().AddNewOption(option);
                go.GetComponentInParent<SelectorManager>().UpdateFilterValues();
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

                    default:
                        print("Error: the requisition " + level + " has not implemented yet");
                        break;
                }
            }
        }
    }

    private void GetWWWAttributes(string base64str)
    {
        _attributes = base64str.Split(","[0]);
        _m.SetAttributes(_attributes);
        
        string request = _requestServerPath + "/row/" + _m.GetDatasetLoaded() + "/0";
        print("Request 2: " + request);
        
        StartCoroutine(GetRequest(request, 1));
    }

    private void GetWWWFirstLine(string base64Str)
    {
        _m.SetDatasetStatus(true);
        _firstRowData = base64Str.Split(',');
        _typeOfAttributes = new string[_firstRowData.Length];


        for (int i = 0; i < _firstRowData.Length; i++)
        {
            if (float.TryParse(_firstRowData[i], NumberStyles.Any, CultureInfo.InvariantCulture, out float _))
            {
                _typeOfAttributes[i] = "CONT";
            }
            else
            {
                _typeOfAttributes[i] = "CAT";
            }
        }

        _m.SetTypeOfAttribute(_typeOfAttributes);

    }

    private void ChangeMenuCollors(RaycastHit hit)
    {
        if(hit.transform.CompareTag("check")) return;
        //change active slice to blue and every other slice to default
        Transform parentObj = hit.transform.parent.transform.parent;
        //check which slice is selected and turn all the others to default color
        foreach (Transform child in parentObj)
        {
            if (child.name != hit.transform.parent.name)
            {
                child.GetComponentInChildren<MeshRenderer>().material = mColors[2];
            }
        }
        //if there is a green slice (dataset selected) don't turn it to blue
        if (hit.transform.GetComponent<MeshRenderer>().material.color != mColors[1].color)
        {
            hit.transform.GetComponent<MeshRenderer>().material = mColors[0];
        }
    }
}
