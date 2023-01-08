﻿using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Collector : MonoBehaviour
{
    public UnityEvent onNoEnergy;

    public AudioController.Audio soundOnCollect;
    
    public float maxLoad = 100f;
    public float keepLoad = 20f;
    private float curLoad;

    private void Start()
    {
        curLoad = keepLoad;
        SetLoad(keepLoad);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (NoEnergy()) return;
        if (col.TryGetComponent(out Resource resource))
        {
            AddLoad(resource.mass);
            Destroy(resource.gameObject);
            
            AudioController.instance.PlaySound(soundOnCollect);
        }
    }

    public void AddLoad(float amount)
    {
        SetLoad(curLoad + amount);
    }
    public void SetLoad(float amount)
    {
        if (NoEnergy()) return;
        curLoad = Mathf.Clamp(amount, 0, maxLoad);
        HudController.Instance.SetLoadPercent(curLoad/maxLoad, keepLoad/maxLoad);
        if (NoEnergy())
        {
            onNoEnergy.Invoke();
        }
    }

    public bool NoEnergy()
    {
        return curLoad <= 0;
    }

    public void Reset()
    {
        curLoad = keepLoad;
    }

    public float GetExcess()
    {
        return Mathf.Max(curLoad - keepLoad, 0);
    }
}
