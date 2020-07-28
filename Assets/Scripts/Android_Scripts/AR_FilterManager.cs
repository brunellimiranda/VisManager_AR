using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AR_FilterManager : DefaultTrackableEventHandler
{
    private Manager _m;
    private RadialMenuBehavior _rm;

    private Dictionary<int, List<string>> _filterValues;
    private List<GameObject> _filter = new List<GameObject>();
    
    private List<string[]> _database;
    private List<string> _databaseLabels;
    private List<string> _attributeTypes;

    private string _filterString;

    public GameObject radialMenuPrefab;
    public GameObject minMaxSliderPrefab;
    public GameObject anchor;
    public GameObject blocker;
    
    private void Awake()
    {
        _m = GameObject.Find("LogicManager").GetComponent<Manager>();
        _rm = GameObject.Find("ftradialmenu").GetComponent<RadialMenuBehavior>();
        _filterValues = new Dictionary<int, List<string>>();
    }

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
        _rm.gameObject.SetActive(true);

        foreach (GameObject f in _filter)
        {
            f.SetActive(false);
        }
        
        if (_m.GetDatasetStatus())
        {
            blocker.SetActive(false);
            _attributeTypes = _m.GetTypeOfAttributes().ToList();
            _databaseLabels = _m.GetAttributes().ToList();
            _rm.gameObject.SetActive(true);
            _rm.SetRadialOptions(_m.GetAttributes().ToList());
            UpdateListOfFilters();
            GenerateFilter();
            return;
        }
        blocker.SetActive(true);
        Debug.LogError("Error 6: Select some dataset before, please!");
    }

    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();
        UpdateListOfFilters();
    }

    // atualiza a string que contém os filtros que serão
    // utilizados para a geração da visualização
    public void GenerateFilter()
    {
        if (_filterValues.Count == 0) return;
        
        _filterString = "[";

        int index = 0;
        
        foreach (var f in _filterValues.Where(f => f.Value != null))
        {
            if (_attributeTypes[f.Key] != "CAT")
            {
                // continuum
                if (Convert.ToBoolean(f.Value[2])) //if seleção invertida
                {
                    _filterString += "{\"not\": {\"field\": \"" + _databaseLabels[f.Key] + "\", ";
                    _filterString += "\"range\":[" + f.Value[0] + "," + f.Value[1] + "]}}";
                }
                else
                {
                    _filterString += "{\"field\": \"" + _databaseLabels[f.Key] + "\", ";
                    _filterString += "\"range\":[" + f.Value[0] + "," + f.Value[1] + "]}";
                }
            }
            else
            {
                _filterString += "{\"field\": \"" + _databaseLabels[f.Key] + "\", ";
                _filterString += "\"oneOf\": [";
                
                for (int j = 0; j < f.Value.Count; j++)
                {
                    _filterString += "\"" + f.Value[j] + "\"";

                    if (j + 1 == f.Value.Count)
                    {
                        _filterString += "]}";
                        break;
                    }

                    _filterString += ",";
                }
            }

            _filterString += ',';
        }
        
        _filterString = _filterString.Substring(0, _filterString.Length-1);
        _filterString += "]";
        print(_filterString);
        _m.SetFilter(_filterString);
    }

    public string GetFilter()
    {
        GenerateFilter();
        return _filterString;
    }
    
    // insere um novo filtro na lista de filtros em 
    // <filtermanager>.UpdateListOfFilters
    // a cada filtro novo criado

    public void UpdateFilter(string option)
    {
        bool exist = false;
        _rm.gameObject.SetActive(false);
        
        // verifica se o filtro requisitado já existe
        if (_filter.Count != 0)
        {
            foreach (var filter in _filter)
            {
                filter.SetActive(false);
                
                if (filter.name != "Filter_" + option) continue;
                filter.SetActive(true);
                exist = true;
            }
        }

        if (exist) return; 
        // se não existe, instancia novo filtro com esses parâmetros
        int index = _m.GetAttributeID(option);
        string url = _m.GetUrlPath() + "/field/" + _m.GetDatasetLoaded() + '/' + option;
        StartCoroutine(GetRequest(url, option, index));
    }

    public void UpdateListOfFilters()
    {
        if (_filter.Count == 0)return;
        foreach (GameObject f in _filter)
        {
            int id = f.GetComponent<SelectorManager>().GetId();
            List<string> filteredParameters = f.GetComponent<SelectorManager>().GetFilterValues();
            
            try
            {
                _filterValues.Add(id, filteredParameters);
            }
            catch (ArgumentException)
            {
                _filterValues[id] = filteredParameters;
            }
        }
    }
    
    public void UpdateListOfFilters(int id, List<string> filteredParameters)
    {
        try
        {
            _filterValues.Add(id, filteredParameters);
        }
        catch (ArgumentException)
        {
            _filterValues[id] = filteredParameters;
        }
    }
    
    IEnumerator GetRequest(string url, string option, int id)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError) print("Error: cannot retrieve information from server");
            else InstantiateNewFilter(webRequest.downloadHandler.text, option, id);
        }
    }

    private void InstantiateNewFilter(string parameters, string label, int id)
    {
        string attType = _m.GetTypeOfAttribute(label);
        string[] values = parameters.Split(',');
        
        GameObject newFilter = attType != "CAT" ? InitializeMinMaxSlider(values, id) : InstantiateNewRadialMenu(values, id);
        newFilter.name = "Filter_" + label;
        _filter.Add(newFilter);
    }
    
    private GameObject InitializeMinMaxSlider(string[] values, int Id)
    {
        Vector2 minMax = _m.GetComponent<ProjectUtils>().GetMinMaxValues(values);
        
        GameObject slider = Instantiate(minMaxSliderPrefab, anchor.transform);
        slider.transform.localPosition = new Vector3(0, 0.2f, 0);
        slider.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
        
        slider.AddComponent<SelectorManager>();

        slider.GetComponent<SliderManager>().SetValues(minMax);
        slider.GetComponent<SliderManager>().ChangeOptionsTag("Numeric_ITem");
        slider.GetComponent<SelectorManager>().SetInformations(Id, "CONT");
        slider.GetComponent<SelectorManager>().UpdateFilterValues();
        
        return slider;
    }
    
    private GameObject InstantiateNewRadialMenu(string[] values, int Id)
    {
        GameObject radialMenu = Instantiate(radialMenuPrefab, anchor.transform);
        radialMenu.transform.localScale = new Vector3(0.0045f, 0.001f, 0.0045f);
      
        radialMenu.AddComponent<SelectorManager>();

        List<string> categories = _m.GetComponent<ProjectUtils>().GetAttributes(values);
        
        radialMenu.GetComponent<RadialMenuBehavior>().SetRadialOptions(categories);
        radialMenu.GetComponent<RadialMenuBehavior>().ChangeFatiaTag("Categoric_Item");
        radialMenu.GetComponent<SelectorManager>().SetInformations(Id, "CAT");
        return radialMenu;
    }
    
    public void DestroyAllFilters()
    {
        foreach (var filter in _filter) Destroy(filter);
        _filter = new List<GameObject>();
        _filterValues = new Dictionary<int, List<string>>();
        _filterString = "";
    }
    
    public void NextPage(int increment)
    {
        _rm.UpdatePage(increment);
    }
}