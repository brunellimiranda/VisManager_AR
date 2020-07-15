using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ScatterPlot : DefaultTrackableEventHandler
{
    public Manager localManager;
    private string serverPath;
    private string datasetName;
    private string[] datasetAtt;
    private string[] datasetTypeOfAtt;

    private string default_X = "";
    private string default_Y = "";
    private string default_Z = "";

    private void Awake()
    {
        localManager = GameObject.Find("LogicManager").GetComponent<Manager>();
    }

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();

        serverPath = localManager.GetUrlPath();
        datasetName = localManager.GetDatasetLoaded();
        if (!string.IsNullOrEmpty(localManager.GetDatasetLoaded()))
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
            if ((cont == 1) && ((datasetTypeOfAtt[i] == "CONT")))
            {
                default_Y = datasetAtt[i];
                cont++;
            }
            if ((cont == 0) && (datasetTypeOfAtt[i] == "CONT"))
            {
                default_X = datasetAtt[i];
                cont++;
            }
            if ((cat == 0) && (datasetTypeOfAtt[i] == "CAT"))
            {
                default_Z = datasetAtt[i];
                cat++;
            }

            if (cont > 1 && cat > 0)
            {
                break;
            }
        }

        print("X: " + default_X);
        print("Y: " + default_Y);
        print("Z: " + default_Z);

        this.transform.GetChild(0).GetComponent<AR_ChartGenerator>().GetChart(serverPath, datasetName, default_X, default_Y, default_Z, "scatterplot",
            this.gameObject.name);
    }
}
