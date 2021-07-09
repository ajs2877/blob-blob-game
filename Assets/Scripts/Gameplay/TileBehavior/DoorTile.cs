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

        foreach (Triggerable triggerObj in allTriggers)
        {
            triggerObj.triggerRecievers.Add(gameObject);
        }

        foreach (Triggerable togglerObj in allTogglers)
        {
            togglerObj.triggerRecievers.Add(gameObject);
        }
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
            List<Vector2Int> doorCoordinates = gameGrid.GetElementLocation(gameObject);
            List<GameObject> objectsAtSpot = new List<GameObject>();
            foreach (Vector2Int doorCoordinate in doorCoordinates)
            {
                objectsAtSpot.AddRange(gameGrid.GetElementsAtLocation(doorCoordinate.x, doorCoordinate.y));
            }
            objectsAtSpot.RemoveAll(element => element == gameObject);

            // Door counts towards this so if it is greater than 1, another object is here at same spot.
            if (objectsAtSpot.Count > 0)
            {
                return;
            }

            StartCoroutine(ChangeDoorState(isOpen));
        }
        else
        {
            bc.isTrigger = isOpen;
            sr.sprite = sprites[isOpen ? 1 : 0];
            gameObject.tag = isOpen ? "notwindblocking" : "Untagged";
        }
    }

    private IEnumerator ChangeDoorState(bool isOpen)
    {
        // Turn on collider to prevent blobs and stuff from moving in and getting stuck
        bc.isTrigger = isOpen;
        
        // make collider itself have no hitbox to make sure blobs and stuff can move out of closing door space without teleporting pushed out.
        Vector2 bcOriginalSize = bc.size;
        bc.size = new Vector2(0, 0);

        // Delay coroutine to make visual change look nicer and some time for exiting blob to leave space cleanly
        yield return new WaitForSeconds(0.2f);
        sr.sprite = sprites[isOpen ? 1 : 0];
        gameObject.tag = isOpen ? "notwindblocking" : "Untagged";

        // Give full collider size back
        bc.size = bcOriginalSize; 

        yield break;
    }
}
