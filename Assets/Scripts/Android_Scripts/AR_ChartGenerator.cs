using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class AR_ChartGenerator : MonoBehaviour
{
    string sender = null;
    GameObject senderObj = null;

   public void GetChart(string path, string dataset, string xAxis, string yAxis, string vis, string senderObj)
    {
        sender = senderObj;
        string url = "";
        url = path + "/generate/" + dataset + "/chartgen.html?x=" +
            xAxis + "&y=" + yAxis + "&chart=" + vis + "&title=" + dataset;

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

        senderObj = GameObject.Find(sender);
        var child = senderObj.transform.GetChild(0);
        SpriteRenderer renderer = child.GetComponent<SpriteRenderer>();
        //SpriteRenderer renderer = this.gameObject.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            renderer = child.gameObject.AddComponent<SpriteRenderer>();
            //renderer = this.gameObject.AddComponent<SpriteRenderer>();
        }
        renderer.sprite = sprite;

        PolygonCollider2D collider = child.GetComponent<PolygonCollider2D>();
        //PolygonCollider2D collider = gameObject.GetComponent<PolygonCollider2D>();
        if(collider == null)
        {
            child.gameObject.AddComponent<PolygonCollider2D>();
            //gameObject.AddComponent<PolygonCollider2D>();
        }

        //child.gameObject.transform.localPosition = new Vector3(0,0,0);
        print("Chegou no fim do generateViz");
    }
}
