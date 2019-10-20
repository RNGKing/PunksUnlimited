using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ResourceTextManager : MonoBehaviour
{
    Text resourceText;
    void Awake()
    {
        resourceText = GetComponent<Text>();
    }

    internal void Character_ResourceUpdated(float CurrentValue)
    {
        if(resourceText != null)
        {
            resourceText.text = CurrentValue.ToString();
        }
    }
}
