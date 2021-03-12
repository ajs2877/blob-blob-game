using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTile : Triggerable
{
    [SerializeField]
    private Sprite[] sprites;
    
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
    
    void OnTriggerStay2D(Collider2D col)
    {
        triggered = true;
        spriteRenderer.sprite = sprites[1];
    }

    void OnTriggerExit2D(Collider2D col)
    {
        triggered = false;
        spriteRenderer.sprite = sprites[0];
    }
}
