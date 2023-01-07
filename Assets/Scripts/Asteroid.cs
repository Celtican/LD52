using System;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float rate = 10;

    public void Update()
    {
        transform.Rotate(0, 0, rate * Time.deltaTime);
    }
}