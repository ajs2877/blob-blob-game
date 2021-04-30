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
    [SerializeField]
    private bool invertDoorState = false;
    public Triggerable[] allTriggers;
    public Triggerable[] allTogglers;

    // Start is called before the first frame update
    void Start()
    {
        bc = gameObject.GetComponent<BoxCollider2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isOpen = invertDoorState;
        if (allTriggers.Length > 0)
        {
            // set the starting state for our checks
            bool validTriggers = !activateOnAnyTrigger;
            foreach (Triggerable triggerObj in allTriggers)
            {
                if (activateOnAnyTrigger)
                {
                    validTriggers = validTriggers || triggerObj.triggered;
                }
                else
                {
                    validTriggers = validTriggers && triggerObj.triggered;
                }
            }
            isOpen = validTriggers;
            if (invertDoorState) isOpen = !isOpen;
        }

        // Inverts the door's state always
        foreach (Triggerable toggleObj in allTogglers)
        {
            if (toggleObj.triggered)
            {
                isOpen = !isOpen;
            }
        }

        bc.isTrigger = isOpen;
        sr.sprite = sprites[isOpen ? 1 : 0];
    }
}
