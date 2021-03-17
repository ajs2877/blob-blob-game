using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitTile : MonoBehaviour
{
    [SerializeField]
    private bool state;
    
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
        // Detects when the boulder is touching
        GameObject gameObjectTouching = col.gameObject;
        if (gameObjectTouching.tag.Equals("boulder"))
        {
            StartCoroutine(IgnoreCollision(col.collider, col.otherCollider));

            float distance = Vector3.Distance(gameObject.transform.position, gameObjectTouching.transform.position);
            if (distance < 0.4f)
            {
                // Remove boulder
                Destroy(gameObjectTouching);

                // Set new texture of pit and remove collision
                spriteRenderer.sprite = sprites[1];
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }

    IEnumerator IgnoreCollision(Collider2D col1, Collider2D col2)
    {
        Physics2D.IgnoreCollision(col1, col2, true);
        while (Physics2D.Distance(col1, col2).isOverlapped) yield return null;
        Physics2D.IgnoreCollision(col1, col2, false);
    }
}
