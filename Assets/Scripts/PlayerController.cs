using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance { get; private set; }
    
    public UnityEvent onLose;

    public float velocityAcceleration = 1f;
    public float velocityMax = 3f;
    public float torqueAcceleration = 45;
    public float torqueMax = 180;
    public float torqueVelocityRatio = 1f; // how much velocity the ship loses while rotating

    public float beamTaxPerSecond = 0.25f;
    public float rotateTaxPerSecond = 0.1f;
    public float speedTaxPerSecond = 0.1f;
    
    private Rigidbody2D body;
    private Collector collector;
    private BeamController beam;
    private Gun gun;
    private new Camera camera;

    private bool lost = false;

    // Update is called once per frame
    private void Awake()
    {
        camera = Camera.main;
        gun = GetComponent<Gun>();
        body = GetComponent<Rigidbody2D>();
        collector = GetComponent<Collector>();
        beam = GetComponentInChildren<BeamController>();

        instance = this;
    }

    public void Update()
    {
        bool isShooting = Input.GetButton("Shoot");
        if (isShooting)
        {
            gun.ShootTowards(camera.ScreenToWorldPoint(Input.mousePosition));
        }
        
        if (!isShooting && Input.GetButton("Tractor"))
        {
            beam.gameObject.SetActive(true);
            collector.AddLoad(-beamTaxPerSecond*Time.deltaTime);
        }
        else
        {
            beam.gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        collector.AddLoad(-Mathf.Abs(Input.GetAxis("Horizontal")) * rotateTaxPerSecond * Time.deltaTime);
        collector.AddLoad(-Mathf.Max(Input.GetAxis("Vertical"), 0) * speedTaxPerSecond * Time.deltaTime);
        
        body.angularVelocity = Mathf.Clamp(body.angularVelocity - Input.GetAxis("Horizontal") * torqueAcceleration, -torqueMax, torqueMax);

        float angle = body.rotation * Mathf.Deg2Rad;
        float magnitude = body.velocity.magnitude;
        magnitude = Mathf.Clamp(
            magnitude + Mathf.Max(Input.GetAxis("Vertical"), -0.5f) * velocityAcceleration
            - Mathf.Abs(body.angularVelocity)/torqueMax*torqueVelocityRatio, 0, velocityMax);
        
        body.velocity = new Vector2(Mathf.Cos(angle)*magnitude, Mathf.Sin(angle)*magnitude);
    }

    public void Lose()
    {
        if (lost) return;

        lost = true;
        onLose.Invoke();
    }

    private void OnDestroy()
    {
        instance = null;
    }
}
