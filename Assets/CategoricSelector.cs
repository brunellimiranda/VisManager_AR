using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

public class CategoricSelector : MonoBehaviour
{
    private string _lastAttType;
    private string _lastAttLabel;
    
    public GameObject blocker;
    public GameObject radialmenu;
    
    private Manager m;
    private ProjectUtils u;
    
    // Start is called before the first frame update
    void Start()
    {
        m = GameObject.Find("LogicManager").GetComponent<Manager>();
        u = GameObject.Find("LogicManager").GetComponent<ProjectUtils>();
    }
    
    public void SetNewAttribute(string label)
    {
        _lastAttLabel = label;
    }

    public void RefreshAttribute()
    {
        _lastAttType = m.GetTypeOfAttribute(_lastAttLabel);

        if (_lastAttType == "CAT")
        {
            blocker.SetActive(false);
            List<string> options = u.GetAttributes(m.GetCategories());
            
            radialmenu.GetComponent<RadialMenuBehavior>().SetRadialOptions(options);
            return;
        }
        blocker.SetActive(true);
    }



    public void NextPage()
    {
        radialmenu.GetComponent<RadialMenuBehavior>().UpdatePage(1);
    }

    public void ClearOptions()
    {
        radialmenu.GetComponent<RadialMenuBehavior>().ClearOptions();
    }


}
