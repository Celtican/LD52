using System;
using UnityEngine;
using UnityEngine.Events;

public class Hull : MonoBehaviour
{
    public UnityEvent onDie;
    
    public float maxHull = 5;
    private float curHull;

    public float damageTakenFromAsteroid = 3;

    public bool isPlayer = false;

    private void Start()
    {
        curHull = maxHull;
        SetHull(maxHull);
    }

    public void Damage(float amount)
    {
        SetHull(curHull - amount);
    }

    public void Heal()
    {
        SetHull(maxHull);
    }

    public void SetHull(float newAmount)
    {
        if (IsDead()) return;

        curHull = Mathf.Clamp(newAmount, 0, maxHull);
        if (isPlayer) HudController.Instance.SetHullPercent(curHull/maxHull);
        if (IsDead()) onDie.Invoke();
    }

    public bool IsDead()
    {
        return curHull <= 0;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<Asteroid>())
        {
            Damage(damageTakenFromAsteroid);
        }
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}