using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RadialMenuBehavior : MonoBehaviour
{
    private Manager m;
    public GameObject[] fatias;

    
    private int _totalPages;
    private int _currentPage;
    private Dictionary<int, List<string>> _pagesValues = new Dictionary<int, List<string>>();
    private List<string> _options;

    private List<string> _selectedOptions;

    private void Start()
    {
        m = GameObject.Find("LogicManager").GetComponent<Manager>();
        _selectedOptions = new List<string>();
        if (_pagesValues.Count == 0)
        {
            _pagesValues.Add(0, new List<string>
            {
                "option", "option", "option", 
                "option", "option", "option", 
                "option", "option", "option", 
                "option", "option", "option"
            });    
        }
    }
    
    
    public void SetRadialOptions(List<string> options)
    {
        _options = new List<string>(options);
        UpdateOptions();
    }

    void UpdateOptions()
    {
        _pagesValues = new Dictionary<int, List<string>>();
        int size = _options.Count;
        _totalPages = (int) Mathf.Floor(size / 12);

        for (int cp = 0; cp <= _totalPages; cp++)
        {
            List<string> values = new List<string>();
            for (int j = 11 * cp; j < 11 * (cp + 1); j++)
            {
                values.Add(j >= _options.Count ? "disable" : _options[j]);
            }
            if (_totalPages != 0) values.Add("more options");
            _pagesValues.Add(cp, values);
        }

        UpdatePage(0);
    }
    

    public void UpdatePage(int increment)
    {
        if (_currentPage + increment > _totalPages) _currentPage = 0;
        else _currentPage += increment;

        
        List<string> options = _pagesValues[_currentPage];
        
        
        for (int i = 0; i < options.Count; i++)
        {
            fatias[i].GetComponentInChildren<Text>().text = options[i];
            fatias[i].gameObject.SetActive(true);
            if (fatias[i].GetComponentInChildren<Text>().text == "disable")
            {
                fatias[i].gameObject.SetActive(false);
            }
            if (_totalPages == 0 && _currentPage == 0) fatias[11].gameObject.SetActive(false);
        }

        foreach (GameObject fatia in fatias)
        {
            if (fatia.GetComponentInChildren<Text>().text == "disable")
            {
                fatia.gameObject.SetActive(false);
            }
        }
    }

    public void ClearOptions()
    {
        _currentPage = 0;
        _totalPages = 0;
        _pagesValues = new Dictionary<int, List<string>>();
        _pagesValues.Add(0, new List<string>
        {
            "option", "option", "option", 
            "option", "option", "option", 
            "option", "option", "option", 
            "option", "option", "option"
        });
        UpdatePage(0);
    }

    public void ChangeFatiaTag(string newTag)
    {
        foreach (GameObject f in fatias)
        {
            f.tag = newTag;
        }
    }
    public List<string> GetSelected()
    {
        return _selectedOptions;
    }

    public void AddNewSelected(string option)
    {
        if (!_selectedOptions.Contains(option))
        {
            _selectedOptions.Add(option);
        }
    }

    public void ClearSelectedOptions()
    {
        _selectedOptions = new List<string>();
    }
    
}
