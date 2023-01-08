using System;
using UnityEngine;

public class PointTowards : MonoBehaviour
{
    public GameObject pointTowards;
    public float distanceToHide = 8;
    public float hideSpeed = 4f;

    private float range;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        range = transform.localPosition.magnitude;
    }

    private void Update()
    {
        if (pointTowards == null) return;
        
        Utils.RotateTowards(gameObject, pointTowards.transform.position);
        
        Vector3 distance = pointTowards.transform.position - transform.parent.position;
        transform.position = (distance.normalized * range) + transform.parent.position;

        Color spriteColor = spriteRenderer.color;
        if (distance.magnitude < distanceToHide)
        {
            spriteColor.a = Mathf.Clamp01(spriteColor.a - (hideSpeed * Time.deltaTime));
        }
        else
        {
            spriteColor.a = Mathf.Clamp01(spriteColor.a + (hideSpeed * Time.deltaTime));
        }

        spriteRenderer.color = spriteColor;
    }
}
