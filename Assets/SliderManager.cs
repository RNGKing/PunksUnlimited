using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    Slider slider;
    // Start is called before the first frame update
    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    internal void Character_EnergyUpdate(float CurrentValue)
    {
        if(slider != null)
        {
            slider.value = CurrentValue;
        }
    }
}
