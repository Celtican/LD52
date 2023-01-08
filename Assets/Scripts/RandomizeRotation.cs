using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomizeRotation : MonoBehaviour
{
    private void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
    }
}
