using System;
using UnityEngine;

public class WarpGate : MonoBehaviour
{
    public GameObject shopScreen;
    public bool alwaysTeleport = false;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<PlayerController>() &&
            (alwaysTeleport || (col.TryGetComponent(out Collector collector) && collector.GetExcess() > 0)))
        {
            Time.timeScale = 0;
            shopScreen.SetActive(true);
        }
    }
}
