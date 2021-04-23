using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitTile : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;
    
    [SerializeField]
    private BoxCollider2D parentCollider; // Set with inspector. GetComponentInParent isn't working.
    private BoxCollider2D pitTriggerCollider;
    private SpriteRenderer spriteRenderer;

    private AudioSource sound;

    void Start()
    {
        pitTriggerCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[0];
        sound = GetComponentInParent<AudioSource>();
    }

    void OnTriggerStay2D(Collider2D col)
    {
        // Detects when the boulder is touching
        GameObject gameObjectTouching = col.gameObject;
        
        if (gameObjectTouching.tag.Equals("moveable"))
        {
            // Will check between boulder and pit to know if boulder has fallen in
            float distance = Vector3.Distance(col.transform.position, pitTriggerCollider.transform.position);
            if (distance < 0.4f)
            {
                // Remove boulder
                Destroy(col.gameObject);

                // Set new texture of pit and remove collision
                spriteRenderer.sprite = sprites[1];
                parentCollider.enabled = false;
                pitTriggerCollider.enabled = false;

                // Play the sound clip
                sound.Play();
            }
        }
    }
}
