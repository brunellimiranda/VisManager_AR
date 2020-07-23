using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vuforia;

public class AttributeSelector : MonoBehaviour
{
    public GameObject reticle;
    public GameObject radialmenu;
    private Manager m;
    
    // Start is called before the first frame update
    void Start()
    {
        m = GameObject.Find("LogicManager").GetComponent<Manager>();
        turnOnReticle();
    }

    // Update is called once per frame
    void Update()
    {
        reticle.SetActive(true);
    }
    
    private void turnOnReticle()
    {
        reticle.SetActive(true);
    }

    public void NextPage()
    {
        radialmenu.GetComponent<RadialMenuBehavior>().UpdatePage(1);
    }

    public void UpdateGrid()
    {
        radialmenu.GetComponent<RadialMenuBehavior>().SetRadialOptions(m.GetAttributes().ToList());
    }

    public void ClearOptions()
    {
        radialmenu.GetComponent<RadialMenuBehavior>().ClearOptions();
    }
}
