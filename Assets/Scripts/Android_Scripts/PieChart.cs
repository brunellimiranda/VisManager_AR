using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class PieChart : DefaultTrackableEventHandler
{
    public Manager localManager;
    private string serverPath;
    private string datasetName;
    private string[] datasetAtt;
    private string[] datasetTypeOfAtt;

    private string default_X = "";
    private string default_Y = "";

    private void Awake()
    {
        localManager = GameObject.Find("LogicManager").GetComponent<Manager>();
    }
    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
        //var tempPreviousTarget = localManager.GetActiveImageTarget();

        //if (tempPreviousTarget != this.gameObject && tempPreviousTarget.tag != "Visualization")
        //{
        //    //does not work when more than 1 card is tracked at the same time
        //    tempPreviousTarget.transform.GetChild(0).gameObject.SetActive(false);
        //}

        ////teste
        //GameObject.Find("LogicManager").GetComponent<Manager>().SetActiveImageTarget(this.gameObject);

        serverPath = localManager.GetUrlPath();
        datasetName = localManager.GetDatasetLoaded();
        if(!string.IsNullOrEmpty(localManager.GetDatasetLoaded()))
        {
            SetDefaultAxis();
        }
    }

    protected override void OnTrackingLost()
    {
        //
    }

    private void SetDefaultAxis()
    {
        datasetAtt = localManager.GetAttributes();
        datasetTypeOfAtt = localManager.GetTypeOfAttributes();
        var cont = 0; var cat = 0;
        for (int i = 0; i < datasetAtt.Length; i++)
        {
            if ((cat == 0) && (datasetTypeOfAtt[i] == "CAT"))
            {
                default_X = datasetAtt[i];
                cat++;
            }
            if ((cont == 0) && (datasetTypeOfAtt[i] == "CONT"))
            {
                default_Y = datasetAtt[i];
                cont++;
            }
            if (cat > 0 && cont > 0)
            {
                break;
            }
        }

        print("X: " + default_X);
        print("Y: " + default_Y);

        this.transform.GetChild(0).GetComponent<AR_ChartGenerator>().GetChart(serverPath, datasetName, default_X, default_Y, "piechart",
            this.gameObject.name);
    }
}
