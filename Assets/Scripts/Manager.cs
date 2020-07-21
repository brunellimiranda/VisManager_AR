using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public string url_path;
    public string datasetLoaded = "";

    public string[] attributes;
    public string lastAttSelected;
    public string[] typeOfAttribute;
    public string[] categoriesOfSelectedAttribute;

    public bool datasetReady;

    //public GameObject activeTarget = null;

    public string GetLastSelected()
    {
        return lastAttSelected;
    }

    public void SetLastSelected(string attLabel)
    {
        lastAttSelected = attLabel;
    }
    
    public string[] GetCategories()
    {
        return categoriesOfSelectedAttribute;
    }

    public void SetCategories(string[] values)
    {
        categoriesOfSelectedAttribute = values;
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
    
    public string GetTypeOfAttribute(string label)
    {
        for (int i = 0; i < typeOfAttribute.Length; i++)
        {
            if (attributes[i] == label) return typeOfAttribute[i];
        }

        return "unknown attribute";
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
