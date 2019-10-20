using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class VictorTextController : MonoBehaviour
{
    public Text victorText;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void SetVictorText(int playerNumber)
    {
        victorText.text = $"PLAYER {playerNumber + 1}";
    }
}
