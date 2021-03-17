using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTile : Triggerable
{
    [SerializeField]
    private bool state;

    [SerializeField]
    private GameObject[] triggerObjects;

    [SerializeField]
    private Sprite[] sprites;

    // To keep track when to start detection player movement
    private bool primed = false;
    private SpriteRenderer spriteRenderer;
    private GameObject objectOn = null;

    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
    
    void OnTriggerStay2D(Collider2D col)
    {
        if (primed && objectOn == col.gameObject)
        {
            // Detects when the blob is exiting and changes state if the blob is going the right direction
            if (Vector3.Distance(gameObject.transform.position, col.gameObject.transform.position) > 0.65f)
            {
                objectOn = null;
                primed = false;
                DirectionVector directionVector = col.gameObject.GetComponent<DirectionVector>();
                Vector2 tileDirection = state ? gameObject.transform.up : gameObject.transform.up * -1;
                if (Vector2.Angle(tileDirection, directionVector.direction) < 45)
                {
                    state = !state;
                    //changes texture to match
                    spriteRenderer.sprite = sprites[state ? 1 : 0];
                    triggered = state;
                }
            }
        }
        // Blob enter far enough inward. Set the switch to be ready for switching.
        else if (Vector2.Distance(gameObject.transform.position, col.gameObject.transform.position) < 0.65f)
        {
            objectOn = col.gameObject;
            primed = true;
        }
    }
}
