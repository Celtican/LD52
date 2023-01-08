using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RotateOverTime : MonoBehaviour
{
    public float maxRate = 1;
    public float minRate = 15;
    private float rate = 10;
    public bool randomSign = true;

    private void Awake()
    {
        rate = Random.Range(minRate, maxRate);
        if (randomSign && Random.Range(0, 2) == 0) rate *= -1;
    }

    public void Update()
    {
        transform.Rotate(0, 0, rate * Time.deltaTime);
    }
}