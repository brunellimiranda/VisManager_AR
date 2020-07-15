using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class AR_ChartGenerator : MonoBehaviour
{
    string sender = null;
    //GameObject senderChild;
    //Transform senderChild;

   public void GetChart(string path, string dataset, string xAxis, string yAxis, string vis, string senderObj)
    {
        sender = senderObj;
        string url = "";
        url = path + "/generate/" + dataset + "/chartgen.html?x=" +
            xAxis + "&y=" + yAxis + "&chart=" + vis + "&title=" + dataset;

        print("Requisition: " + url);
        
        StartCoroutine(GetRequest(url));
    }

    public void GetChart(string path, string dataset, string xAxis, string yAxis, string zAxis, string vis, string senderObj)
    {
        sender = senderObj;
        string url = "";
        url = path + "/generate/" + dataset + "/chartgen.html?x=" +
            xAxis + "&y=" + yAxis + "&z=" + zAxis + "&chart=" + vis + "&title=" + dataset;

        print("Requisition: " + url);

        StartCoroutine(GetRequest(url));
    }



    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                print("Error: cannot retrieve information from server");
            }
            else generateViz(webRequest.downloadHandler.text);
        }
    }

    void generateViz(string base64str)
    {
        string Base64string = base64str;
        byte[] Bytes = Convert.FromBase64String(base64str);
        Texture2D tex = new Texture2D(900, 465);
        tex.LoadImage(Bytes);
        //até aqui ok, provavelmente mudar a partir daqui
        Rect rect = new Rect(0, 0, tex.width, tex.height);
        Sprite sprite = Sprite.Create(tex, rect, new Vector2(0, 0), 100f);

        var senderChild = this.gameObject;

        SpriteRenderer renderer = senderChild.GetComponent<SpriteRenderer>();

        if (renderer == null)
        {
            renderer = senderChild.gameObject.AddComponent<SpriteRenderer>();

        }
        renderer.sprite = sprite;

        PolygonCollider2D collider = senderChild.GetComponent<PolygonCollider2D>();

        if (collider == null)
        {
            senderChild.gameObject.AddComponent<PolygonCollider2D>();

        }


    }
}
