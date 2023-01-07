using System;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public float mass = 1f;

    private bool untethered = false;
    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    public void MoveTowards(GameObject target, float speed)
    {
        if (!untethered)
        {
            untethered = true;
            transform.SetParent(null, true);
            body.simulated = true;
        }
        Vector2 distance = target.transform.position - transform.position;
        body.velocity = distance.normalized * speed;
    }
}