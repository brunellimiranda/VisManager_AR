using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class SliderManager : MonoBehaviour
{
    private Vector3 _leftBoundary;
    private Vector3 _rightBoundary;
    
    [Header("Limites")]
    public float MinLimit;
    public float MaxLimit;

    [Header("Valores")]
    public float MinValue;
    public float MaxValue;
    public bool isInvertedSelection;
    
    [Header("Speed")]
    public float speed = 0.05f;

    [Header("Game Objects")]
    public GameObject rect;
    public GameObject minValue;
    public GameObject maxValue;
    public GameObject checkBox;
    
    [Header("Text")]
    public TextMesh MinText;
    public TextMesh MaxText;
     
    
    float endLimit = 9;
    // Start is called before the first frame update

    public void Update()
    {
        GetValues();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        MinText.text = MinValue.ToString("0.00", CultureInfo.InvariantCulture);
        MaxText.text = MaxValue.ToString("0.00", CultureInfo.InvariantCulture);
        
        if (!Physics.Raycast(ray, out hit)) return;
        if (!Input.GetMouseButton(0)) return;
    
        if (hit.transform.CompareTag("SliderButton")) UpdateValues(hit.transform);
    }

    public void UpdateValues(Transform t)
    {
        float minPosition = minValue.transform.localPosition.x + 0.1f;
        float maxPosition = maxValue.transform.localPosition.x - 0.1f;
        Vector3 position = new Vector3();
        float speed = 0.05f;

        switch (t.name)
        {
            case "MinUpButton":
                position = minValue.transform.localPosition;
                position += new Vector3(speed, 0f, 0f);
                position.x = Mathf.Clamp(position.x, -endLimit, maxPosition);
                minValue.transform.localPosition = position;
                break;

            case "MinDownButton":
                position = minValue.transform.localPosition;
                position += new Vector3(-speed, 0f, 0f);
                position.x = Mathf.Clamp(position.x, -endLimit, maxPosition);
                minValue.transform.localPosition = position;
                    
                break;

            case "MaxUpButton":
                position = maxValue.transform.localPosition;
                position += new Vector3(speed, 0f, 0f);
                position.x = Mathf.Clamp(position.x, minPosition, endLimit);
                maxValue.transform.localPosition = position;
                break;

            case "MaxDownButton":
                position = maxValue.transform.localPosition;
                position += new Vector3(-speed, 0f, 0f);
                position.x = Mathf.Clamp(position.x, minPosition, endLimit);
                maxValue.transform.localPosition = position;
                break;
        }
    }

    public Vector2 GetOnlyValues()
    {
        float a = minValue.transform.localPosition.x;
        float b = maxValue.transform.localPosition.x;
        
        float nA = (a - -endLimit) / (endLimit - -endLimit);
        float nB = (b - -endLimit) / (endLimit - -endLimit);

        MinValue = MinLimit + nA * (MaxLimit - MinLimit);
        MaxValue = MinLimit + nB * (MaxLimit - MinLimit);
        
        return new Vector2(MinValue, MaxValue);
    }
    
    public List<string> GetValues()
    {
        Vector2 values = GetOnlyValues();

        List<string> state = new List<string>
        {
            values.x.ToString(CultureInfo.InvariantCulture), 
            values.y.ToString(CultureInfo.InvariantCulture),
            isInvertedSelection.ToString()
        };
        return state;
    }
    

    public void SetValues(float min, float max)
    {
        MinLimit = min;
        MaxLimit = max;
    }
    
    public void SetValues(Vector2 minMax)
    {
        MinLimit = minMax.x;
        MaxLimit = minMax.y;
    }

    public void ChangeOptionsTag(string newTag)
    {
    rect.tag = newTag;
    minValue.tag = newTag;
    maxValue.tag = newTag;
    }

    public void UpdateCheckVis(bool inverted)
    {
        isInvertedSelection = inverted;
        if (isInvertedSelection)
            checkBox.GetComponent<Renderer>().material.color = Color.green;
        else
            checkBox.GetComponent<Renderer>().material.color = Color.red;
    }
}
