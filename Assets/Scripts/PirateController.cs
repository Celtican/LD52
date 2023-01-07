using System;
using UnityEngine;

public class PirateController : MonoBehaviour
{
    public float speed = 4f;
    public float rotationLerp = 0.2f;
    public float shootRange = 7f;
    private Gun gun;
    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        gun = GetComponent<Gun>();
    }

    private void Start()
    {
        if (PlayerController.instance != null) Utils.RotateTowards(gameObject, PlayerController.instance.transform.position);
    }

    private void Update()
    {
        if (PlayerController.instance == null) return;
        Vector3 targetPosition = PlayerController.instance.transform.position;
        Vector2 difference = targetPosition - transform.position;
        
        Utils.RotateTowards(gameObject, targetPosition, rotationLerp * Time.deltaTime);
        if (difference.magnitude <= shootRange) gun.ShootTowards(targetPosition);
    }

    private void FixedUpdate()
    {
        
        float angle = body.rotation * Mathf.Deg2Rad;
        
        body.velocity = new Vector2(Mathf.Cos(angle)*speed, Mathf.Sin(angle)*speed);
    }
}
