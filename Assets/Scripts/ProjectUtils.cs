﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ProjectUtils : MonoBehaviour
{
    public Vector2 GetMinMaxValues(int index, List<string[]> database)
    {
        List<string> values = GetAttributes(index, database);

        float min = float.MaxValue;
        float max = float.MinValue;

        foreach (string value in values)
        {
            Double fValue = float.Parse(value);

            if (fValue < min)
                min = (float) fValue;
            else if (fValue > max)
                max = (float) fValue;
        }
        
        return new Vector2(min, max);
    }
    
    public Vector2 GetMinMaxValues(string[] attributes)
    {
        float min = float.MaxValue;
        float max = float.MinValue;

        foreach (string value in attributes)
        {
            float fValue = float.Parse(value, CultureInfo.InvariantCulture);
            if (fValue < min)
                min = (float) fValue;
            else if (fValue > max)
                max = (float) fValue;
        }
        
        return new Vector2(min, max);
    }

    public List<string> GetAttributes(int index, List<string[]> database)
    {
        List<string> attributes = new List<string>();

        foreach (string[] line in database)
        {
            if (!attributes.Contains(line[index]))
                attributes.Add(line[index]);
        }

        return attributes;
    }
    
    public List<string> GetAttributes(string[] attList)
    {
        List<string> attributes = new List<string>();

        foreach (string a in attList)
        {
            if (!attributes.Contains(a))
                attributes.Add(a);
        }

        return attributes;
    }

    public List<string> GetAttributes(string row)
    {
        List<string> categoricAttributes = new List<string>();
        string[] attributes = row.Split(',');
        
        foreach (string cat in attributes)
        {
            if (!categoricAttributes.Contains(cat))
                categoricAttributes.Add(cat);
        }

        
        return categoricAttributes;
    }
    
    public List<string> GetCategoricAttributes(List<Type> tipos, List<string> labels)
    {
        List<string> onlyCategoric = new List<string>();
        for (int i = 0; i < labels.Count; i++)
        {
            if (tipos[i] == typeof(string))
                onlyCategoric.Add(labels[i]);
        }

        return onlyCategoric;
    }

    public List<string> GetContinuumAtributes(List<Type> tipos, List<string> labels)
    {
        List<string> onlyContinuum = new List<string>();
        for (int i = 0; i < labels.Count; i++)
        {
            if (tipos[i] != typeof(string))
                onlyContinuum.Add(labels[i]);
        }

        return onlyContinuum;
    }

    public int GetIndexOfDropdownOption(string dpdOption, List<string> labels)
    {
        for (int i = 0; i < labels.Count; i++)
        {
            if (labels[i] == dpdOption)
                return i;
        }

        return 404;
    }

    public string ConvertToSingleString(List<string> line)
    {
        string phrase = "";

        for (int i = 0; i < line.Count; i++)
        {
            phrase += line[i];
            if (i + 1 < line.Count)
                phrase += ",";
            else
                return phrase;
        }

        return phrase;
    }
}
