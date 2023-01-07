using System;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float timeUntilDestroy = 5f;

    private void Update()
    {
        timeUntilDestroy -= Time.deltaTime;
        
        if (timeUntilDestroy <= 0) Destroy(gameObject);
    }
}