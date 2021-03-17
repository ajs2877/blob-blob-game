using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitTile : MonoBehaviour
{
    [SerializeField]
    private bool filled = false;
    
    [SerializeField]
    private Sprite[] sprites;
    
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[0];
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if(!filled)
        {
            // Detects when the boulder is touching
            GameObject gameObjectTouching = col.gameObject;
            if (gameObjectTouching.tag.Equals("boulder"))
            {
                // Begin dedicated checking between pit and boulder that does not block OnCollisionStay2D
                StartCoroutine(IgnoreCollision(col.collider, col.otherCollider));
            }
        }
    }

    IEnumerator IgnoreCollision(Collider2D boulderCollider, Collider2D pitCollider)
    {
        // Turn off collision between boulder and pit so boulder can be moved over pit
        Physics2D.IgnoreCollision(boulderCollider, pitCollider, true);

        // Will keep checking between boulder and pit to know if boulder has fallen in
        while (!filled && boulderCollider != null && Physics2D.Distance(boulderCollider, pitCollider).isOverlapped)
        {
            float distance = Vector3.Distance(boulderCollider.transform.position, pitCollider.transform.position);
            if (distance < 0.4f)
            {
                // Remove boulder
                Destroy(boulderCollider.gameObject);

                // Set new texture of pit and remove collision
                spriteRenderer.sprite = sprites[1];
                pitCollider.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                filled = true;
            }
            yield return null;
        }

        // If boulder wasn't destroyed but moved out of pit area, turn on collisions so we can detect if it comes back
        if(boulderCollider != null)
        {
            Physics2D.IgnoreCollision(boulderCollider, pitCollider, false);
        }
    }
}
