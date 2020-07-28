using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorManager : MonoBehaviour
{
    private string _type;
    private int _id;
    private List<string> _filterValues;
    private bool invertedSelection;
    
    public void SetInformations(int id, string tipo)
    {
        _id = id;
        _type = tipo;
    }

    public void SetSelectorType(string tipo)
    {
        _type = tipo;
    }
    
    public string GetSelectorType()
    {
        return _type;
    }
    
    public void SetId(int index)
    {
        _id = index;
    }
    
    public int GetId()
    {
        return _id;
    }

    public List<string> GetFilterValues()
    {
        return _filterValues;
    }

    public void UpdateFilterValues()
    {
        _filterValues = _type == "CAT" ? GetComponent<RadialMenuBehavior>().GetSelected() : GetComponent<SliderManager>().GetValues();
        GetComponentInParent<AR_FilterManager>().UpdateListOfFilters(_id, _filterValues);
    }
    
    public void NextPage(int increment)
    {
        GetComponent<RadialMenuBehavior>().UpdatePage(increment);
    }

    public void AddNewOption(string o)
    {
        GetComponent<RadialMenuBehavior>().AddNewSelected(o);
    }

    public void ClearAllOptions()
    {
        GetComponent<RadialMenuBehavior>().ClearSelectedOptions();
    }

    public void SetInvertedSelection()
    {
        invertedSelection = !invertedSelection;
    }
    
    
    
}
