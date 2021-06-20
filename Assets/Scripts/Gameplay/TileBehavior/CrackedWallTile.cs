using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackedWallTile : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private BoxCollider2D parentCollider; // Set with inspector. GetComponentInParent isn't working.
    private BoxCollider2D wallTriggerCollider;
    private SpriteRenderer spriteRenderer;
    private int size;

    private AudioSource sound;
    
    void Start()
    {
        wallTriggerCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[0];
        sound = GetComponentInParent<AudioSource>();
        size = GetComponentInParent<GridObject>().size;
    }


    void OnTriggerStay2D(Collider2D col)
    {
        // Detects when the big blob is touching
        GameObject gameObjectTouching = col.gameObject;

        if (gameObjectTouching.name.Equals("PurpleBigBlob"))
        {
            // Will check between wall and big blob to know if smashed
            float distance = Vector3.Distance(col.transform.position, wallTriggerCollider.transform.position);
            if (distance < 0.7f * size)
            {
                DestroyWall();
            }
        }
    }

    public void DestroyWall()
    {
        // Set new texture of wall and remove collision
        spriteRenderer.sprite = sprites[1];
        parentCollider.enabled = false;
        wallTriggerCollider.enabled = false;
        parentCollider.gameObject.GetComponent<AllowOnlyBigBlobIn>().enabled = false;

        // Play the sound clip
        sound.Play();
    }
}
