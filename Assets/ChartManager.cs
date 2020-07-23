using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using Vuforia;

public class ChartManager : DefaultTrackableEventHandler
{
    private Manager _m;
    private RadialMenuBehavior _rm;
    
    
    private string _serverPath;
    private string _datasetName;
    private string[] _datasetAtt;
    private string[] _datasetTypeOfAtt;

    private string _defaultX = "";
    private string _defaultY = "";
    private string _defaultZ = "";

    private string _chartType = "";
    private string _chartAttTypes = "";
    
    private int _maxDimensions = 0;
    private int _necessaryDimensions = 0;
    private int _selectedDimensions = 0;
    
    private bool _visSelected = false;
    
    // Dimentions are:
    // X - Categoric (c)
    // Y - Number (n)
    // Z - Color (c)

    void Awake()
    {
        _m = GameObject.Find("LogicManager").GetComponent<Manager>();
        _rm = GetComponentInChildren<RadialMenuBehavior>();
    }
    
    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();

        _serverPath = _m.GetUrlPath();
        _datasetName = _m.GetDatasetLoaded();
        if (!string.IsNullOrEmpty(_m.GetDatasetLoaded()))
        {
            if (_visSelected)
            {
                _rm.gameObject.SetActive(false);
                SetDefaultAxis();    
            }
            else
            {
                _rm.gameObject.SetActive(true);
            }
            
        }
        else Debug.LogError("Error01: Please, select one dataset using a Dataset Card!");
    }

    protected override void OnTrackingLost()
    {
        //
    }

    private void SetDefaultAxis()
    {
        _datasetAtt = _m.GetAttributes();
        _datasetTypeOfAtt = _m.GetTypeOfAttributes();
        
        int cont = 0;
        int cat = 0;
        
        switch (_chartType)
        {
           case "scatterplot":
               for (int i = 0; i < _datasetAtt.Length; i++)
               {
                   if (_datasetTypeOfAtt[i] != "CONT") continue;
                   
                   switch (cont)
                   {
                       case 0:
                           _defaultX = _datasetAtt[i];
                           cont++;
                           break;
                       case 1:
                           _defaultY = _datasetAtt[i];
                           cont++;
                           break;
                   }
                   if(cont == 2) break;
                   if(i + 1 == _datasetAtt.Length && (cont < 2)) 
                       Debug.LogError("Error2: Please, select another dataset for this chart!");
               }

               GetComponentInChildren<AR_ChartGenerator>().
                   GetChart(_serverPath, _datasetName, _defaultX, _defaultY,_chartType);
               break;
           
           case "heatmap":
               for (int i = 0; i < _datasetAtt.Length; i++)
               {
                   if (_datasetTypeOfAtt[i] != "CAT") continue;
                   
                   switch (cont)
                   {
                       case 0:
                           _defaultX = _datasetAtt[i];
                           cont++;
                           break;
                       case 1:
                           _defaultY = _datasetAtt[i];
                           cont++;
                           break;
                   }
                   if(cont == 2) break;
                   if(i + 1 == _datasetAtt.Length && (cont < 2)) 
                       Debug.LogError("Error2: Please, select another dataset for this chart!");
               }

               GetComponentInChildren<AR_ChartGenerator>().
                   GetChart(_serverPath, _datasetName, _defaultX, _defaultY,_chartType);
               break;
           
           case "parallel_coordinates":
               string fold = "";
               for (int i = 0; i < _datasetAtt.Length; i++)
               {
                   if (_datasetTypeOfAtt[i] == "CAT" && cat == 0)
                   {
                       _defaultZ = _datasetAtt[i];
                       cat++;
                   }

                   if (_datasetTypeOfAtt[i] == "CONT" && cont < 3)
                   {
                       _defaultX = _datasetAtt[i];
                       fold += _defaultX;
                       cont++;
                       if(cont == 3 && cat == 1) break;
                       fold += ";";
                   }
                                      
                   if(i + 1 == _datasetAtt.Length && (cont < 3 || cat == 0)) 
                       Debug.LogError("Error2: Please, select another dataset for this chart!");
               }
               
               GetComponentInChildren<AR_ChartGenerator>().
                   GetChart(_serverPath, _datasetName, fold, _defaultZ, _chartType);
               break;

           default:
                // barchart, piechart, linechart
               
                for (int i = 0; i < _datasetAtt.Length; i++)
                {
                    if(cat == 0 && _datasetTypeOfAtt[i] == "CAT")
                    {
                        _defaultX = _datasetAtt[i];
                        cat++;
                    }
                    if(cont == 0 && _datasetTypeOfAtt[i] == "CONT")
                    {
                        _defaultY = _datasetAtt[i];
                        cont++;
                    }
                    
                    if(cat > 0 && cont > 0) break;
                    if(i + 1 == _datasetAtt.Length && (cat == 0 || cont == 0)) 
                        Debug.LogError("Error2: Please, select another dataset for this chart!");
                }
                
                GetComponentInChildren<AR_ChartGenerator>().
                    GetChart(_serverPath, _datasetName, _defaultX, _defaultY,_chartType);
                break;
            
        }
    }

    public void SetVisType(string vis)
    {
        _chartType = vis;
        _rm.gameObject.SetActive(false);
        
        switch (_chartType)
        {
            case "barchartvertical":
                _chartAttTypes = "cn";
                _necessaryDimensions = 2;
                _maxDimensions = 2;
                break;
            
            case "piechart":
                _chartAttTypes = "cn";
                _necessaryDimensions = 2;
                _maxDimensions = 2;
                break;
            
            
            case "scatterplot":
                _chartAttTypes = "nnc";
                _necessaryDimensions = 2;
                _maxDimensions = 3;
                break;
            
            case "heatmap":
                _chartAttTypes = "cc";
                _necessaryDimensions = 2;
                _maxDimensions = 2;
                break;
            
            case "parallel_cordinates":
                _chartAttTypes = "fold";
                _necessaryDimensions = 2;
                _maxDimensions = 10;
                break;
        }
        
        _visSelected = true;
    }

    public void ClearVisType()
    {
        _chartType = "";
        _visSelected = false;
    }
}
