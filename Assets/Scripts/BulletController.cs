using System;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public bool isPlayer = false;
    
    public float speed = 10f;
    [SerializeField] private float damage = 1;

    public float velocityInheritancePercent = 0.5f;

    public void ShootTowards(Vector2 target, Vector2 inheritVelocity)
    {
        Utils.RotateTowards(gameObject, target);
        GetComponent<Rigidbody2D>().velocity = ((new Vector3(target.x, target.y) - transform.position).normalized * speed) + new Vector3(inheritVelocity.x, inheritVelocity.y)*velocityInheritancePercent;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out Hull hull) && hull.isPlayer != isPlayer)
        {
            hull.Damage(damage);
            Destroy(gameObject);
        }
    }
}
