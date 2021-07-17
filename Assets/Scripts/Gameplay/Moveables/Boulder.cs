using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : Moveables
{
    [SerializeField]
    private Sprite[] sprites;

    private int lavaFloorTouching = 0;
    private SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[0];
    }

    void Update()
    {
        UpdateIsMoving();

        if (!isMoving && wasMoving)
        {
            NotifyListeningTiles(true);
        }

        UpdateWasMoving();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Lava"))
        {
            lavaFloorTouching++;
            spriteRenderer.sprite = sprites[1];
            spriteRenderer.color = new Color(220, 175, 140);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Lava"))
        {
            lavaFloorTouching--;
            if (lavaFloorTouching == 0)
            {
                spriteRenderer.sprite = sprites[0];
                spriteRenderer.color = new Color(176, 149, 128);
            }
        }
    }
}
