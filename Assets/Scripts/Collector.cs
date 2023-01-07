using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Collector : MonoBehaviour
{
    public UnityEvent onNoEnergy;
    
    public float maxLoad = 20f;
    public float keepLoad = 5f;
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
}