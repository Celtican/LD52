using System;
using UnityEngine;
using UnityEngine.Events;

public class Hull : MonoBehaviour
{
    public static float loadOnDestroy = 0;
    
    public UnityEvent onDie;
    public AudioController.Audio audioOnHit;
    public AudioController.Audio audioOnDie;
    public GameObject hitParticlePrefab;
    
    public float maxHull = 5;
    private float curHull;

    public float damageTakenFromAsteroid = 3;
    public LayerMask asteroidLayer;

    public bool isPlayer = false;
    private PlayerController playerController;

    private void Start()
    {
        curHull = maxHull;
        SetHull(maxHull);
        playerController = GetComponent<PlayerController>();
    }

    public void Damage(float amount)
    {
        if (IsDead()) return;
        AudioController.instance.PlaySound(audioOnHit);
        Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
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
        if (IsDead())
        {
            onDie.Invoke();
            AudioController.instance.PlaySound(audioOnDie);
            if (!isPlayer)
            {
                Collector.instance.AddLoad(loadOnDestroy);
            }
        }
    }

    public bool IsDead()
    {
        return curHull <= 0;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (asteroidLayer == (asteroidLayer | (1 << col.gameObject.layer)))
        {
            Damage(damageTakenFromAsteroid);
            if (isPlayer)
            {
                playerController.Flip();
            }
        }
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}