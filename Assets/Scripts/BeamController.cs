using System;
using UnityEngine;

public class BeamController : MonoBehaviour
{
    public float absorbSpeed = 5f;
    private new Camera camera;

    private void Awake()
    {
        camera = Camera.main;
    }

    private void OnEnable()
    {
        RotateTowardsMouse();
    }

    private void Update()
    {
        transform.localPosition = new Vector3();

        RotateTowardsMouse();
    }

    private void RotateTowardsMouse()
    {
        Utils.RotateTowards(gameObject, camera.ScreenToWorldPoint(Input.mousePosition));
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.TryGetComponent<Resource>(out Resource resource))
        {
            resource.MoveTowards(transform.parent.gameObject, absorbSpeed);
        }
    }
}