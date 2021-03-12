using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTile : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;

    private BoxCollider2D bc;
    private SpriteRenderer sr;

    [SerializeField]
    private bool activateOnAnyTrigger = false;
    public Triggerable[] allTriggers;

    // Start is called before the first frame update
    void Start()
    {
        bc = gameObject.GetComponent<BoxCollider2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isOpen = false;
        if (allTriggers.Length > 0)
        {
            // set the starting state for our checks
            isOpen = !activateOnAnyTrigger; 
            foreach (Triggerable triggerObj in allTriggers)
            {
                if (activateOnAnyTrigger)
                {
                    isOpen = isOpen || triggerObj.triggered;
                }
                else
                {
                    isOpen = isOpen && triggerObj.triggered;
                }
            }
        }

        bc.isTrigger = isOpen;
        sr.sprite = sprites[isOpen ? 1 : 0];
    }
}
