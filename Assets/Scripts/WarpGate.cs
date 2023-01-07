using System;
using UnityEngine;

public class WarpGate : MonoBehaviour
{
    public GameObject shopScreen;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<PlayerController>())
        {
            Time.timeScale = 0;
            shopScreen.SetActive(true);
        }
    }
}