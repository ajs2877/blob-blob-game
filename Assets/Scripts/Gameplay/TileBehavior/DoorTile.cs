﻿using System.Collections;
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

    public GameObject dotPrefab;
    private List<GameObject> spawnedDots = new List<GameObject>();
    private Color inactive = new Color(0.3113208f, 0.3113208f, 0.3113208f, 0.6941177f);
    private Color active = new Color(1, 1, 1, 0.83f);


    // Start is called before the first frame update
    void Start()
    {
        gameGrid = GameObject.Find("GameController").GetComponent<TrueGrid>();
        bc = gameObject.GetComponent<BoxCollider2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        bool isLarge = GetComponent<GridObject>().size == 2;

        int i = 0;
        foreach (Triggerable triggerObj in allTriggers)
        {
            triggerObj.triggerRecievers.Add(gameObject);
            SpawnDot(-0.2f + (i % 2) * 0.4f, -0.2f + ((i / 2) * 0.4f), isLarge);
            i++;
        }

        foreach (Triggerable togglerObj in allTogglers)
        {
            togglerObj.triggerRecievers.Add(gameObject);
            SpawnDot(-0.2f + (i % 2) * 0.4f, ((i / 2) * 0.4f), isLarge);
            i++;
        }
    }

    private void SpawnDot(float xOffset, float yOffset, bool isLarge)
    {
        if (!dotPrefab) return;
        if (isLarge)
        {
            xOffset *= 2;
            yOffset *= 2;
        }

        GameObject dot = Instantiate(dotPrefab, transform.position, transform.rotation);
        dot.transform.Translate(new Vector3(xOffset, yOffset, -0.1f));
        if(isLarge) dot.transform.localScale *= 2;
        dot.transform.SetParent(transform);
        spawnedDots.Add(dot);
    }


    // Update is called once per frame
    void Update()
    {
        bool isOpen = invertDoorState;
        int triggeredTriggers = 0;
        if (allTriggers.Length > 0)
        {
            // set the starting state for our checks
            bool validTriggers = !activateOnAnyTrigger;
            foreach (Triggerable triggerObj in allTriggers)
            {
                if (triggerObj.triggered)
                {
                    triggeredTriggers++;
                }

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
                triggeredTriggers++;
            }
        }

        for(int index = 0; index < spawnedDots.Count; index++)
        {
            if(index < triggeredTriggers)
            {
                spawnedDots[index].GetComponent<SpriteRenderer>().color = active;
            }
            else
            {
                spawnedDots[index].GetComponent<SpriteRenderer>().color = inactive;
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
            if (isOpen)
            {
                foreach (GameObject dot in spawnedDots)
                {
                    dot.SetActive(false);
                }
            }
            else
            {
                foreach (GameObject dot in spawnedDots)
                {
                    dot.SetActive(true);
                }
            }
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
