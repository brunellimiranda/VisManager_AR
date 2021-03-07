using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.UI;

public class AR_ChartGenerator : MonoBehaviour
{
    public InputField b64printer;
    public void GetChart(string path, string dataset, string xaxis, string yaxis, string vis, string title, string filter)
    {
        string url;
        
        if (vis == "parallel_coordinates") url = path + "/generate/" + dataset + "/chartgen.html?fold=" +
                                                xaxis + "&z=" + yaxis + "&chart=parallel_coordinates&title=" + dataset;

        else url = path + "/generate/" + dataset + "/chartgen.html?x=" +
                   xaxis + "&y=" + yaxis + "&chart=" + vis + "&title=" + title + "&filter=" + filter;

        print("Request Chart: " + url);
        StartCoroutine(GetRequest(url));
    }

    public void GetChart(string path, string dataset, string xAxis, string yAxis, string zAxis, string vis, string title, string filter)
    {
        string url;
        url = path + "/generate/" + dataset + "/chartgen.html?x=" +
            xAxis + "&y=" + yAxis + "&z=" + zAxis + "&chart=" + vis + "&title=" + title + "&filter=" + filter;

        print("Request Chart: " + url);

        StartCoroutine(GetRequest(url));
    }

    public void GetChart2(string requisition)
    {
        StartCoroutine(GetRequest(requisition));
    }

    public void GetChart(string requisition)
    {
        print("Load State with Request Chart " + requisition);
        GenerateViz(requisition);
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
            else GenerateViz(webRequest.downloadHandler.text);
        }
    }

    void GenerateViz(string base64Str)
    {
        GameObject senderChild = gameObject;
        SpriteRenderer renderer = senderChild.GetComponent<SpriteRenderer>();
        PolygonCollider2D collider = senderChild.GetComponent<PolygonCollider2D>();
        
        if (base64Str.Contains("<title>"))
        {
            string[] temptext = base64Str.Split(',');

            base64Str = temptext[1];
            temptext = base64Str.Split('\'');
            base64Str = temptext[0];
            
            b64printer.text = base64Str;
            print(base64Str);
        }
        
        byte[] Bytes = Convert.FromBase64String(base64Str);
        Texture2D tex = new Texture2D(900, 465);
        tex.LoadImage(Bytes);

        Rect rect = new Rect(0, 0, tex.width, tex.height);
        Sprite sprite = Sprite.Create(tex, rect, new Vector2(0, 0), 100f);
        
        if (renderer == null) renderer = senderChild.gameObject.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;

        if (collider == null) senderChild.gameObject.AddComponent<PolygonCollider2D>();
        b64printer.text += " success";
    }

    public void ClearChart()
    {
        Destroy(gameObject.GetComponent<SpriteRenderer>());
        Destroy(gameObject.GetComponent<PolygonCollider2D>());
        
    }
}
