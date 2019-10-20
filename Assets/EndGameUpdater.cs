using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameUpdater : MonoBehaviour
{
    public Text text;

    int maxTime = 180;

    public void SetMaxTime(int maxTime)
    {
        this.maxTime = maxTime;
    }

    public void UpdateTime(float val)
    {
        text.text = $"REMAINING TIME : {maxTime - (int)val}";
    }

}
