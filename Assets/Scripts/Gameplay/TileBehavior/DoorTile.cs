using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTile : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;

    private BoxCollider2D bc;
    private SpriteRenderer sr;
    private TrueGrid gameGrid;

    [SerializeField]
    private bool activateOnAnyTrigger = false;
    [SerializeField]
    private bool invertDoorState = false;
    public Triggerable[] allTriggers;
    public Triggerable[] allTogglers;

    // Start is called before the first frame update
    void Start()
    {
        gameGrid = GameObject.Find("GameController").GetComponent<TrueGrid>();
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

        // Do not close door if there is an object blocking the door
        if(!isOpen && bc.isTrigger)
        {
            Vector2Int doorCoordinate = gameGrid.GetGridSpace(gameObject, false);
            List<GameObject> objectsAtSpot = gameGrid.GetElementsAtLocation(doorCoordinate.x, doorCoordinate.y);

            // Door counts towards this so if it is greater than 1, another object is here at same spot.
            if(objectsAtSpot.Count > 1)
            {
                return;
            }
        }

        bc.isTrigger = isOpen;
        sr.sprite = sprites[isOpen ? 1 : 0];
    }
}
