using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCardManager : DefaultTrackableEventHandler
{
    private Manager _m;
    private RadialMenuBehavior _rm;

    private List<string> _options = new List<string>{"Clear All Vis", "Clear One Vis", "Clear Vis Axis", "Clear Filter"};

// Start is called before the first frame update
    void Awake()
    {
        _m = GameObject.Find("LogicManager").GetComponent<Manager>();
        _rm = GetComponentInChildren<RadialMenuBehavior>();
    }

    protected override void OnTrackingFound()
    {
        _rm.gameObject.SetActive(true);
        _options = new List<string>{"Clear All Vis", "Clear One Vis", "Clear Vis Axis", "Clear Filter"};
        _rm.SetRadialOptions(_options);
    }

    protected override void OnTrackingLost()
    {
        _rm.ChangeFatiaTag("ClearOption_Item");
    }

    public void ClearOption(string option)
    {
        switch (option)
        {
            case "Clear All Vis":
                _m.RemoveAllVisualization();
                _rm.gameObject.SetActive(false);
                break;
            
            case "Clear One Vis":
                _rm.SetRadialOptions(_m.GetVisOnScene());
                _rm.ChangeFatiaTag("ClearVis_Item");
                break;
            
            case "Clear Vis Axis":
                _rm.SetRadialOptions(_m.GetVisOnScene());
                _rm.ChangeFatiaTag("ClearAxis_Item");
                break;
            
            case "Clear Filter":
                _m.ClearAllFilter();
                Debug.LogError("Error5: Filter has not yet been implemented. Please, select another option!");
                break;
        }
    }

    public void ClearVis(int id)
    {
        _m.RemoveSomeVisualization(id);
    }

    public void ClearAxis(int id)
    {
        _m.ClearVisAxis(id);
    }
}
