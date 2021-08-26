using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DirectionVector;

public class WindTile : MonoBehaviour
{
    protected TrueGrid gameGrid;
    public AudioSource sound;
    public GameObject windPrefab;
    private GameObject currentWind = null;
    private SpriteRenderer currentWindSpriteRenderer = null;
    private float distanceForWind = -1;
    float moveSpeed = 1.75f;
    int recheckDistanceTimer = 0;
    float delayTimer;

    void Start()
    {
        gameGrid = GameObject.Find("GameController").GetComponent<TrueGrid>();
        delayTimer = Random.value;
    }

    void Update()
    {
        recheckDistanceTimer--;
        if (recheckDistanceTimer < 0)
        {
            DIRECTION direction = GetDirection(gameObject.transform.up);
            Vector2Int tileDirection = GetOffset(direction);
            Vector2Int currentTilePos = gameGrid.GetGridSpace(gameObject, false);
            Vector2Int currentCheckPos = currentTilePos + tileDirection;

            // Regular loop to prevent any possible issue of infinite loop with while loop
            for (int i = 0; i < 100; i++)
            {
                if (!gameGrid.PositionisWithinGrid(currentCheckPos.x, currentCheckPos.y)) break;
                List<GameObject> objectsInPath = gameGrid.GetElementsAtLocation(currentCheckPos.x, currentCheckPos.y);
                foreach (GameObject objectInPath in objectsInPath)
                {
                    if (!objectInPath.CompareTag("notwindblocking"))
                    {
                        i = 100;
                        break;
                    }
                }
                currentCheckPos += tileDirection;

                // if this fires, we have a BIG problem....
                // It means the game kept checking forever in front of wind tile and never hit a wall.
                Debug.Assert(i != 99);
            }

            Vector2 finalTilePos = gameGrid.GetWorldSpace(currentCheckPos);
            distanceForWind = Mathf.Abs((finalTilePos.x - transform.position.x) + (finalTilePos.y - transform.position.y)) - 2f;

            recheckDistanceTimer = 30;
        }

        if (currentWind)
        {
            float distanceTraveled = (currentWind.transform.position - transform.position).magnitude;
            if (distanceTraveled > distanceForWind)
            {
                if(currentWindSpriteRenderer.color.a <= 0)
                {
                    Destroy(currentWind);
                    currentWindSpriteRenderer = null;
                }
                else
                {
                    Color color = currentWindSpriteRenderer.color;
                    currentWindSpriteRenderer.color = new Color(
                        color.r,
                        color.g,
                        color.b,
                        color.a - 0.0075f
                    );
                }
            }
            else
            {
                currentWind.transform.position = currentWind.transform.position + (gameObject.transform.up * moveSpeed * Time.deltaTime);

                float halfThreshold = distanceForWind / 2;
                float absProgress = Mathf.Abs(distanceTraveled - halfThreshold);
                float opacity = Mathf.Pow((halfThreshold - absProgress) / halfThreshold, 0.5f);
                Color color = currentWindSpriteRenderer.color;
                currentWindSpriteRenderer.color = new Color(
                    color.r,
                    color.g,
                    color.b,
                    opacity
                );
            }
        }
        else if(distanceForWind > 2f && delayTimer <= 0)
        {
            delayTimer = Random.value;
            currentWind = Instantiate(windPrefab, transform.position + (gameObject.transform.up * 0.5f), transform.rotation);
            currentWindSpriteRenderer = currentWind.GetComponentInChildren<SpriteRenderer>();
            Color color = currentWindSpriteRenderer.color;
            currentWindSpriteRenderer.color = new Color(
                color.r,
                color.g,
                color.b,
                0
            );
        }
        else
        {
            delayTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Will notify all objects in its path so they can be attempted to be blown.
    /// Call this when any object is moved.
    /// </summary>
    public static void NotifyObjectsInPath(GameObject objectOnPathToFind, List<WindTile> windTiles, TrueGrid gameGrid)
    {
        HashSet<GameObject> objectsOnSameWind = new HashSet<GameObject>();

        foreach (WindTile windTile in windTiles)
        {
            HashSet<GameObject> objectsOnWind = new HashSet<GameObject>();
            DIRECTION direction = GetDirection(windTile.gameObject.transform.up);
            Vector2Int tileDirection = GetOffset(direction);
            Vector2Int currentTilePos = gameGrid.GetGridSpace(windTile.gameObject, false);
            Vector2Int currentCheckPos = currentTilePos + tileDirection;

            // Regular loop to prevent any possible issue of infinite loop with while loop
            for (int i = 0; i < 100; i++)
            {
                if (!gameGrid.PositionisWithinGrid(currentCheckPos.x, currentCheckPos.y)) break;
                List<GameObject> objectsInPath = gameGrid.GetElementsAtLocation(currentCheckPos.x, currentCheckPos.y);
                foreach (GameObject objectInPath in objectsInPath)
                {
                    objectsOnWind.Add(objectInPath);
                    if (!objectInPath.CompareTag("notwindblocking"))
                    {
                        i = 100;
                        break;
                    }
                }
                currentCheckPos += tileDirection;

                // if this fires, we have a BIG problem....
                // It means the game kept checking forever in front of wind tile and never hit a wall.
                Debug.Assert(i != 99);
            }

            // update their wind distance
            Vector2 finalTilePos = gameGrid.GetWorldSpace(currentCheckPos);
            windTile.distanceForWind = Mathf.Abs((finalTilePos.x - windTile.transform.position.x) + (finalTilePos.y - windTile.transform.position.y)) - 2f;

            if (objectsOnWind.Contains(objectOnPathToFind))
            {
                objectsOnSameWind.UnionWith(objectsOnWind);
            }
        }

        if (objectsOnSameWind.Contains(objectOnPathToFind))
        {
            HashSet<GameObject> notifiedNeighbors = new HashSet<GameObject>();
            foreach (GameObject objectInPath in objectsOnSameWind)
            {
                NotifyAnyMoveables(objectInPath, notifiedNeighbors, objectOnPathToFind);
            }
        }
    }

    private static void NotifyAnyMoveables(GameObject neighbor, HashSet<GameObject> notifiedNeighbors, GameObject ignoreGameObject)
    {
        if (neighbor != ignoreGameObject && !notifiedNeighbors.Contains(neighbor) && neighbor.TryGetComponent(out Moveables moveable))
        {
            if (!moveable.isMoving)
            {
                moveable.NotifyListeningTiles(false);
                notifiedNeighbors.Add(neighbor);
            }
        }
    }

    /// <summary>
    /// Will check if it can push object if it is able to and object is in path of wind and unblocked
    /// </summary>
    /// <returns>Whether the object was pushed by wind</returns>
    public bool TryWindPushingObject(GameObject slider)
    {
        // Cannot push 2x2 stuff
        if (slider.GetComponent<GridObject>().size == 2) return false;

        // Detect if object is in front of wind tile and then try to push it
        DIRECTION direction = GetDirection(gameObject.transform.up);
        Vector2Int tileDirection = GetOffset(direction);
        Vector2Int currentTilePos = gameGrid.GetGridSpace(gameObject, false);
        Vector2Int sliderPos = gameGrid.GetGridSpace(slider, false);

        // By checking if the spot in front of the tile at same distance is the same spot as the incoming object, we will know if they are in front
        Vector2Int diff = sliderPos - currentTilePos;
        int magnitude = (int)diff.magnitude;
        Vector2Int spotInFrontOfTile = currentTilePos + (tileDirection * magnitude);
        bool objectIsInFront = spotInFrontOfTile == sliderPos;

        if (objectIsInFront)
        {
            // Check downward in direction to see if the wind is unblocked to the object to push
            Vector2Int currentCheckPos = currentTilePos + tileDirection;
            while (currentCheckPos != sliderPos)
            {
                List<GameObject> objectsInPath = gameGrid.GetElementsAtLocation(currentCheckPos.x, currentCheckPos.y);
                foreach (GameObject objectInPath in objectsInPath)
                {
                    if (!objectInPath.CompareTag("notwindblocking"))
                    {
                        return false;
                    }
                }
                currentCheckPos += tileDirection;
            }

            if (gameGrid.CanMoveElement(slider, true, false, direction))
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Will push object if it is able to and object is in path of wind and unblocked
    /// </summary>
    /// <returns>Whether the object was pushed by wind</returns>
    public bool WindPushObject(GameObject slider)
    {
        // Cannot push 2x2 stuff
        if (slider.GetComponent<GridObject>().size == 2) return false;

        // Detect if object is in front of wind tile and then try to push it
        DIRECTION direction = GetDirection(gameObject.transform.up);
        Vector2Int tileDirection = GetOffset(direction);
        Vector2Int currentTilePos = gameGrid.GetGridSpace(gameObject, false);
        Vector2Int sliderPos = gameGrid.GetGridSpace(slider, false);


        // By checking if the spot in front of the tile at same distance is the same spot as the incoming object, we will know if they are in front
        Vector2Int diff = sliderPos - currentTilePos;
        int magnitude = (int)diff.magnitude;
        Vector2Int spotInFrontOfTile = currentTilePos + (tileDirection * magnitude);
        bool objectIsInFront = spotInFrontOfTile == sliderPos;

        if (objectIsInFront)
        {
            // Check downward in direction to see if the wind is unblocked to the object to push
            Vector2Int currentCheckPos = currentTilePos + tileDirection;
            while (currentCheckPos != sliderPos)
            {
                List<GameObject> objectsInPath = gameGrid.GetElementsAtLocation(currentCheckPos.x, currentCheckPos.y);
                foreach (GameObject objectInPath in objectsInPath)
                {
                    if (!objectInPath.CompareTag("notwindblocking"))
                    {
                        return false;
                    }
                }
                currentCheckPos += tileDirection;
            }

            if (gameGrid.CanMoveElement(slider, true, false, direction))
            {
                Moveables moveable = slider.GetComponent<Moveables>();

                // Let players scripts control their own moving checks
                if (slider.tag.Equals("Player"))
                {
                    PlayerController player = slider.GetComponent<PlayerController>();
                    player.MovePlayer(direction, true, false);
                    moveable.isSliding = true;


                }
                // move non-player stuff if possible and the size of the object is 1 tile
                else
                {
                    gameGrid.MoveElement(slider, false, direction);
                    moveable.isSliding = true;
                }

                if (!sound.isPlaying) sound.Play();
                return true;
            }
        }
        return false;
    }
}
