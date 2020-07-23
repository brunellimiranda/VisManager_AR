using System.Collections;
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

    private string _previousDatasetName;
    private string _newDatasetName;

    private float _timer = 0.0f;
    private float dwellTime = 1.5f;

    private string _requestDataset;
    private string _requestServerPath;

    private string[] _attributes;
    private string[] _typeOfAttributes;
    private string[] _firstRowData;
    
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
        Ray ray = Camera.main.ScreenPointToRay(center);
        RaycastHit hit;
        
        if (!Physics.Raycast(ray, out hit)) return;
        
        Transform go = hit.transform;
        ChangeMenuCollors(hit);
        
        string request1;
        switch (hit.transform.parent.tag)
        {
        

            case "LoadDataset_Item":
                _newDatasetName = go.GetComponentInChildren<UnityEngine.UI.Text>().text;
                if (_previousDatasetName != null)
                {
                    if (_previousDatasetName == _newDatasetName)
                    {
                        _timer += Time.deltaTime;
                        if (!(_timer > dwellTime)) return;
                        _timer = 0;
                        //turn selected slice to green
                        go.GetComponent<MeshRenderer>().material = mColors[1];
                        print("acertou base: " + go.GetComponentInChildren<UnityEngine.UI.Text>().text);
                        _m.SetDatasetLoaded(_newDatasetName);
                        
                        _requestDataset = _m.GetDatasetLoaded();
                        _requestServerPath = _m.GetUrlPath();
                        request1 = _requestServerPath + "/attributes/" + _requestDataset;
                        print("Resquest 1: " + request1);
                        StartCoroutine(GetRequest(request1, 0));
                        string request2 = _requestServerPath + "/row/" + _requestDataset + "/0";
                        print("Resquest 2: " + request2);
                        StartCoroutine(GetRequest(request2, 1));
                        return;
                    }
                }
                _previousDatasetName = _newDatasetName;
                GameObject.Find("Filter_Target").GetComponent<AttributeSelector>().ClearOptions();
                break;
                
            case "LoadAttribute_Item":
                _timer += Time.deltaTime;
                if (!(_timer > dwellTime)) return;
                _timer = 0;
                
                //turn selected slice to green
                go.GetComponent<MeshRenderer>().material = mColors[1];
                print("acertou atributo: " + go.GetComponentInChildren<UnityEngine.UI.Text>().text);

                if (go.GetComponentInChildren<UnityEngine.UI.Text>().text == "more options")
                {
                    GameObject.Find("Filter_Target").GetComponent<AttributeSelector>().NextPage();
                    return;
                }
                
                _requestDataset = _m.GetDatasetLoaded();
                _requestServerPath = _m.GetUrlPath();
                string requestAttribute = go.GetComponentInChildren<UnityEngine.UI.Text>().text;
                request1 = _requestServerPath + "/field/" + _requestDataset + "/" + requestAttribute;

                _m.SetLastSelected(requestAttribute);
                GameObject.Find("SetX_Target").GetComponent<CategoricSelector>().SetNewAttribute(requestAttribute);
                
                print("Resquest 3: " + request1);
                StartCoroutine(GetRequest(request1, 2));
                break;
            
            case "LoadCategoricOptions_Item":
                _timer += Time.deltaTime;
                if (!(_timer > dwellTime)) return;
                _timer = 0;

                //turn selected slice to green
                go.GetComponent<MeshRenderer>().material = mColors[1];
                print("selecionou categoria: " + go.GetComponentInChildren<UnityEngine.UI.Text>().text);
                
                if (go.GetComponentInChildren<UnityEngine.UI.Text>().text == "more options")
                {
                    GameObject.Find("SetX_Target").GetComponent<CategoricSelector>().NextPage();
                }
                
                break;
            
            case "VisOption_Item":
                _timer += Time.deltaTime;
                if (!(_timer > dwellTime)) return;
                _timer = 0;
                
                //turn selected slice to green
                go.GetComponent<MeshRenderer>().material = mColors[1];
                go.GetComponentInParent<ChartManager>().SetVisType(go.GetComponentInChildren<UnityEngine.UI.Text>().text);
                
                print("acertou vis: " + go.GetComponentInChildren<UnityEngine.UI.Text>().text);
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
        _m.SetCategories(base64.Split(','));
        GameObject.Find("SetX_Target").GetComponent<CategoricSelector>().RefreshAttribute();
    }

    private void GetWWWAttributes(string base64str)
    {
        _attributes = base64str.Split(","[0]);
        _m.SetAttributes(_attributes);
        GameObject.Find("Filter_Target").GetComponent<AttributeSelector>().UpdateGrid();

    }

    private void GetWWWFirstLine(string base64str)
    {
        _firstRowData = base64str.Split(',');
        _typeOfAttributes = new string[_firstRowData.Length];

        int i = 0;

        foreach (string x in _firstRowData)
        {
            if (float.TryParse(x, NumberStyles.Any, CultureInfo.InvariantCulture, out float temp))
            {
                _typeOfAttributes[i] = "CONT";
            }
            else
            {
                _typeOfAttributes[i] = "CAT";
            }
            i++;
        }

        _m.SetTypeOfAttribute(_typeOfAttributes);
        _m.SetDatasetStatus(true);
    }

    private void ChangeMenuCollors(RaycastHit hit)
    {
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
