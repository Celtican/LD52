using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomizeSprite : MonoBehaviour
{
    public Sprite[] sprites;
    public bool randomizeX = true;
    public bool randomizeY = true;

    private void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        if (randomizeX && Random.Range(0, 2) == 0) spriteRenderer.flipX = true;
        if (randomizeY && Random.Range(0, 2) == 0) spriteRenderer.flipY = true;
    }
}