using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeUpdate : MonoBehaviour
{
    private DateTime lastTime;

    public string Name;

    public float timeInterval = 1f;

    public event EventHandler<float> OnTick;
    public event EventHandler<float> CurrentTimeUpdated;

    public bool Active = true;

    public float CurrentTime;

    public bool SingleUse = false;
    

    private void Start()
    {
        lastTime = DateTime.UtcNow;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Active)
        {
            CurrentTime += Time.deltaTime;
            CurrentTimeUpdated?.Invoke(this, CurrentTime);
            if (CurrentTime >= timeInterval)
            {
                if (!SingleUse)
                {
                    OnTick?.Invoke(this, CurrentTime);
                    CurrentTime = 0f;
                }
                else
                {
                    OnTick?.Invoke(this, CurrentTime);
                    Active = false;
                }
            }
        }
    }

    public void Reset()
    {
        CurrentTime = 0f;
        Active = true;
    }
}
