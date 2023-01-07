using System;
using UnityEngine;

public class Gun: MonoBehaviour
{
    public GameObject bulletPrefab;
    public float timeBetweenShots = 1;
    private float timeUntilNextShot;
    private Rigidbody2D body;

    public float energyTax = 0.5f;
    private Collector collector;

    private void Awake()
    {
        collector = GetComponent<Collector>();
        body = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        if (timeUntilNextShot > 0)
        {
            timeUntilNextShot = Mathf.Max(timeUntilNextShot - Time.deltaTime, 0);
        }
    }

    public void ShootTowards(Vector2 position)
    {
        if (timeUntilNextShot > 0) return;
        timeUntilNextShot = timeBetweenShots;
        
        Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<BulletController>().ShootTowards(position, body != null ? body.velocity : Vector2.zero);

        if (collector != null)
        {
            collector.AddLoad(-energyTax);
        }
    }
}