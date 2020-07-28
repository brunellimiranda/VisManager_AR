using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    
    // ===== dataset ===== //
    public string url_path;
    private string datasetLoaded;

    // ===== attributes ===== //
    private string[] _attributes;
    private string lastAttSelected;
    private string[] typeOfAttribute;
    
    // ===== Targeted Attributes  ===== //
    private List<string> _categoricAttributes;
    private List<string> _numericAttributes;

    // ===== filtro ===== //
    private string _filter;
    
    public bool datasetReady;

    private Dictionary<int, string> _activeVisualizations = new Dictionary<int, string>();
    private bool _hasSelectedVis = false;

    
    public List<string> GetCategoricAttributes()
    {
        return _categoricAttributes;
    }
    
    public List<string> GetNumericAttributes()
    {
        return _numericAttributes;
    }

    public string GetUrlPath()
    {
        return url_path;
    }

    public void SetUrlPath(string pathAd)
    {
        url_path = pathAd;
    }

    public string GetDatasetLoaded()
    {
        return datasetLoaded;
    }

    public void SetDatasetLoaded(string name)
    {
        datasetLoaded = name;
    }

    public string[] GetAttributes()
    {
        return _attributes;
    }

    public int GetAttributeID(string label)
    {
        for (int i = 0; i < _attributes.Length; i++)
        {
            if (_attributes[i] == label)
                return i;
        }

        return 404;
    }

    public void SetAttributes(string[] data)
    {
        _attributes = data;
    }

    public string[] GetTypeOfAttributes()
    {
        return typeOfAttribute;
    }
    
    public string GetTypeOfAttribute(string label)
    {
        for (int i = 0; i < typeOfAttribute.Length; i++)
        {
            if (_attributes[i] == label) return typeOfAttribute[i];
        }

        return "Error: unknown attribute";
    }

    public void SetTypeOfAttribute(string[] data)
    {
        typeOfAttribute = data;
        
        _categoricAttributes = new List<string>();
        _numericAttributes = new List<string>();
        
        for (int i = 0; i < typeOfAttribute.Length; i++)
        {
            if (typeOfAttribute[i] == "CAT") _categoricAttributes.Add(_attributes[i]);
            else _numericAttributes.Add(_attributes[i]);
        }
        
    }
     //Checks if user already selected a dataset
    public bool GetDatasetStatus()
    {
        return datasetReady;
    }

    public void SetDatasetStatus(bool status)
    {
        datasetReady = status;
    }

    public void AddNewActiveVisualization(int id, string visType)
    {
        _hasSelectedVis = true;
        
        try
        {
            _activeVisualizations.Add(id, visType);
        }
        catch (ArgumentException)
        {
            _activeVisualizations[id] = visType;
        }
    }
    
    public void RemoveSomeVisualization(int id)
    {
        _activeVisualizations.Remove(id);
        GameObject.Find("Chart_Target_0" + id).GetComponent<ChartManager>().ClearVisType();

        if (_activeVisualizations.Count == 0)
        {
            _hasSelectedVis = false;
        }
    }
    
    public void RemoveAllVisualization()
    {
        foreach (var vis in _activeVisualizations)
        {
            GameObject.Find("Chart_Target_0" + vis.Key).GetComponent<ChartManager>().ClearVisType();
        }
        
        _activeVisualizations = new Dictionary<int, string>();
        _hasSelectedVis = false;
    }

    public List<string> GetVisOnScene()
    {
        List<string> activeVis = new List<string>();
        
        foreach (var vis in _activeVisualizations)
        {
            activeVis.Add(vis.Key + " " + vis.Value);
        }

        return activeVis;
    }

    public bool HasActiveVis()
    {
        return _hasSelectedVis;
    }

    public void ClearVisAxis(int id)
    {
        GameObject.Find("Chart_Target_0" + id).GetComponent<ChartManager>().ClearVisAxis();
    }

    public void SetFilter(string config)
    {
        _filter = config;
    }

    public string GetFilter()
    {
        _filter = GameObject.Find("Filter_Target").GetComponent<AR_FilterManager>().GetFilter();
        return _filter;
    }

    public void ClearAllFilter()
    {
        GameObject.Find("Filter_Target").GetComponent<AR_FilterManager>().DestroyAllFilters();
        _filter = "";
    }
}
