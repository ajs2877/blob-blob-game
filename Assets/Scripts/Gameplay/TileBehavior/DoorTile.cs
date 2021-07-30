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
    private List<WindTile> windTiles = new List<WindTile>();

    public GameObject doorTextPrefab;
    public GameObject dotPrefab;
    private List<GameObject> spawnedOverlays = new List<GameObject>();
    private Color inactive = new Color(0.3113208f, 0.3113208f, 0.3113208f, 0.6941177f);
    private Color active = new Color(1, 1, 1, 0.83f);


    // Start is called before the first frame update
    void Start()
    {
        gameGrid = GameObject.Find("GameController").GetComponent<TrueGrid>();
        windTiles.AddRange(FindObjectsOfType(typeof(WindTile)) as WindTile[]);
        bc = gameObject.GetComponent<BoxCollider2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        bool isLarge = GetComponent<GridObject>().size == 2;

        int attachedObjs = 0;
        foreach (Triggerable triggerObj in allTriggers)
        {
            triggerObj.triggerRecievers.Add(gameObject);
            attachedObjs++;
        }

        foreach (Triggerable togglerObj in allTogglers)
        {
            togglerObj.triggerRecievers.Add(gameObject);
            attachedObjs++;
        }

        SpawnText(0, 0, isLarge, attachedObjs.ToString());

        /*
        for(int i = 0; i < attachedObjs; i++)
        {
            float xOffset = attachedObjs <= 1 ? 0 : -0.2f;
            float yOffset = attachedObjs <= 2 ? 0 : -0.2f;
            SpawnDot(xOffset + (i % 2) * 0.4f, yOffset + ((i / 2) * 0.4f), isLarge);
        }
        */
    }

    private void SpawnText(float xOffset, float yOffset, bool isLarge, string text)
    {
        if (!doorTextPrefab) return;

        GameObject textObj = Instantiate(doorTextPrefab, transform.position, new Quaternion());
        textObj.transform.Translate(new Vector3(xOffset, yOffset, -0.1f));
        if (!isLarge) textObj.transform.localScale /= 2;
        textObj.transform.SetParent(transform);
        textObj.transform.Rotate(new Quaternion().eulerAngles, Space.Self);
        textObj.GetComponent<TMPro.TextMeshPro>().text = text;
        spawnedOverlays.Add(textObj);
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
        spawnedOverlays.Add(dot);
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

        for(int index = 0; index < spawnedOverlays.Count; index++)
        {
            SpriteRenderer renderer = spawnedOverlays[index].GetComponent<SpriteRenderer>();
            if (!renderer) continue;
            if (index < triggeredTriggers)
            {
                renderer.color = active;
            }
            else
            {
                renderer.color = inactive;
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
        else if (isOpen && !bc.isTrigger)
        {
            bc.isTrigger = isOpen;
            ActivateWindTiles();

            sr.sprite = sprites[isOpen ? 1 : 0];
            gameObject.tag = isOpen ? "notwindblocking" : "Untagged";
            if (isOpen)
            {
                foreach (GameObject dot in spawnedOverlays)
                {
                    dot.SetActive(false);
                }
            }
            else
            {
                foreach (GameObject dot in spawnedOverlays)
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

    private void ActivateWindTiles()
    {
        // make surrounding tiles be updated so they can be pushed by wind in case our movement opened up a new path
        foreach (WindTile windTile in windTiles)
        {
            windTile.NotifyObjectsInPath(gameObject);
        }
    }
}
