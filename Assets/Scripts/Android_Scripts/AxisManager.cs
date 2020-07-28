using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class AxisManager : DefaultTrackableEventHandler
{
    private Manager _m;
    private RadialMenuBehavior _rm;
    private ChartManager visCard;
    private int _axisID;
    private Text instrutor;

    private string[] _instrutorSteps = {"Select Vis", "Select Axis", "Select Attribute"};

    public GameObject blocker;
    
    // Start is called before the first frame update
    protected void Awake()
    {
        _m = GameObject.Find("LogicManager").GetComponent<Manager>();
        _rm = GetComponentInChildren<RadialMenuBehavior>();
        instrutor = GameObject.Find("C_Instrutor").GetComponentInChildren<Text>();
    }

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();

        bool activeVis = _m.HasActiveVis();
        // 1. Has Active Vis On Scene?
        if (!activeVis)
        {
            blocker.SetActive(true);
            Debug.LogError("Error3: Please, select some one chart before configure axis!");
            instrutor.text = "PLEASE SELECT SOME VIS BEFORE!" + "\nPLEASE SELECT SOME VIS BEFORE!";
            return;
        }
        
        //2. If Yes, get List of visualizations
        blocker.SetActive(false);
        List<string> allVisualizations = _m.GetVisOnScene();
        _rm.SetRadialOptions(allVisualizations);
        instrutor.text = _instrutorSteps[0] + "\n" + _instrutorSteps[1] + "\n" + _instrutorSteps[2];
    }

    protected override void OnTrackingLost()
    {
        _rm.ChangeFatiaTag("AxisVisOption_Item");
        visCard = null;
        _axisID = 0;
        _instrutorSteps = new[] {"Select Vis", "Select Axis", "Select Attribute"};
    }
    
    public void SelectedVis(string optionName)
    {
        visCard = GameObject.Find("Chart_Target_0" + optionName[0]).GetComponent<ChartManager>();
        int maxDimensions = visCard.GetMaxDim();
        List<string> AxisOptions = new List<string>();

        _instrutorSteps[0] = optionName;
        instrutor.text = _instrutorSteps[0] + "\n" + _instrutorSteps[1] + "\n" + _instrutorSteps[2];

        for (int i = 0; i < maxDimensions; i++)
        {
            switch (i)
            {
                case 0:
                    AxisOptions.Add(visCard.GetChartType() == "parallel_coordinates" ? "Vertical Axis" : "X Axis");
                    break;

                case 1:
                    AxisOptions.Add(visCard.GetChartType() == "parallel_coordinates" ? "Color" : "Y Axis");
                    break;

                case 2:
                    AxisOptions.Add("Color");
                    break;
            }
        }


        _rm.SetRadialOptions(AxisOptions);
        _rm.ChangeFatiaTag("AxisOption_Item");
    }

    public void SelectedAxis(string axisName, bool isParallelCoordinates)
    {
        string _attTypes = visCard.GetAttTypes();
        _rm.ChangeFatiaTag("AxisAttribute_Item");
        
        _instrutorSteps[1] = axisName;
        instrutor.text = _instrutorSteps[0] + "\n" + _instrutorSteps[1] + "\n" + _instrutorSteps[2];
        
        switch (axisName)
        {
            case "X Axis":
                _axisID = 0;
                break;
            
            case "Y Axis":
                _axisID = 1;
                break;
            
            case "Color":
                _axisID = isParallelCoordinates ? 1 : 2;
                break;
        }
        
        if (_attTypes[_axisID] == 'n')
        {
            _rm.SetRadialOptions(_m.GetNumericAttributes());
            return;
        }
        
        _rm.SetRadialOptions(_m.GetCategoricAttributes());
    }

    public void SetAxisOnVis(string attributeName)
    {
        string option;
        switch (_axisID)
        {
            case 0:
                option = visCard.SetXAxis(attributeName);
                break;
            
            case 1:
                option = visCard.SetYAxis(attributeName);
                break;
            
            case 2:
                option = visCard.SetZAxis(attributeName);
                break;
            
            default:
                option = "Error";
                break;
        }
        
        _instrutorSteps[2] = option;
        instrutor.text = _instrutorSteps[0] + "\n" + _instrutorSteps[1] + "\n" + _instrutorSteps[2];
    }
    
    public void NextPage()
    {
        _rm.GetComponent<RadialMenuBehavior>().UpdatePage(1);
    }
}
