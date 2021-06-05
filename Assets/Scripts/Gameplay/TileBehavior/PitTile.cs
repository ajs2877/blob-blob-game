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
    public bool filled;

    void Start()
    {
        pitTriggerCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[0];
        sound = GetComponentInParent<AudioSource>();
        filled = false;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        // Detects when the boulder is touching
        GameObject gameObjectTouching = col.gameObject;
        
        if (!filled && gameObjectTouching.tag.Equals("moveable") && gameObjectTouching.name.Contains("Boulder"))
        {
            // Will check between boulder and pit to know if boulder has fallen in if same size
            float distance = Vector3.Distance(col.transform.position, pitTriggerCollider.transform.position);
            if (distance < 0.4f && gameObjectTouching.GetComponent<GridObject>().size == transform.parent.GetComponent<GridObject>().size)
            {
                // Remove boulder
                Destroy(col.gameObject);
                FillPit();

                // Play the sound clip
                sound.Play();
            }
        }
    }

    public void FillPit()
    {
        // Set new texture of pit and remove collision
        spriteRenderer.sprite = sprites[1];
        parentCollider.enabled = false;
        pitTriggerCollider.enabled = false;
        filled = true;
    }
}
