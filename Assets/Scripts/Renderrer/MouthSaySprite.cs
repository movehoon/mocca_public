using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthSaySprite : MonoBehaviour
{
    public SpriteRenderer spriteRenderer = null;

	// Use this for initialization
	void Start ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update ()
    {

		
	}

    internal void SetSprte(Sprite sprite)
    {
        if (spriteRenderer == null) return;
        if (sprite == null) return;

        spriteRenderer.sprite = sprite;
    }
}
