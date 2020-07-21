using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Text;

/// <summary>
/// Script for behaviour of send button located on ARCamera --> Canvas
/// For quick tests purpose, bypass this by deactivating the canvas gameobject in the hierarchy
///     and inputing "http://localhost:3000" in LogicManager -> Manager.cs --> Url_Path public field in the inspector
/// </summary>

public class SendButtonBehaviour : MonoBehaviour
{
    public GameObject panel;

    public InputField tField;
    public Text tText;
    public string iFText;
    public void ShowMessage()
    {
        //print(tField.text);
        iFText = tField.text.ToString();
        StartCoroutine(GetRequest(iFText));
        //tText.text = "Connected to " + tField.text.ToString();
        //canvasI.SetActive(false);
    }

    //Request the data in the server
    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                tText.color = Color.red;
                //tText.text = "Error: " + webRequest.error.ToString();
                tText.text = "Error: " + "Cannot connect to destination host";
            }
            else
            {
                GetWWWText(webRequest.downloadHandler.text);
            }
        }
    }

    //Creates a little pause before deactivating the canvas
    IEnumerator UpdateCoroutine()
    {
        yield return new WaitForSeconds(2);
        panel.SetActive(false);
    }

    //Check the received data
    private void GetWWWText(string base64str)
    {
        print(base64str);
        if (base64str.Contains("chartgen"))
        {
            tText.color = Color.green;
            tText.text = ("Connected to " + iFText);

            GameObject theManager = GameObject.Find("LogicManager");
            Manager managerLocal = theManager.GetComponent<Manager>();

            managerLocal.SetUrlPath(iFText);

            StartCoroutine(UpdateCoroutine());
        }
        else if (base64str.Contains("404"))
        {
            tText.color = Color.red;
            tText.text = ("Error: Server not online");
        }
        //if (base64str != null) return ("Connected to " + iFText);
        else
        {
            tText.color = Color.red;
            tText.text = ("Error: Connected to wrong server");
        }
    }

}
