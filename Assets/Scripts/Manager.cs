using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public string url_path;
    public string datasetLoaded = "";

    public string[] attributes;
    public string[] typeOfAttribute;

    public bool datasetReady;

    //public GameObject activeTarget = null;

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
        return attributes;
    }

    public void SetAttributes(string[] data)
    {
        attributes = data;
    }

    public string[] GetTypeOfAttributes()
    {
        return typeOfAttribute;
    }

    public void SetTypeOfAttribute(string[] data)
    {
        typeOfAttribute = data;
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

    //public GameObject GetActiveImageTarget()
    //{
    //    return activeTarget;
    //}

    //public void SetActiveImageTarget(GameObject target)
    //{
    //    activeTarget = target;
    //}
}
