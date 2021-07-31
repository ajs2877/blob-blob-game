using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DirectionVector;

public class WindTile : MonoBehaviour
{
    protected TrueGrid gameGrid;
    public AudioSource sound;

    void Start()
    {
        gameGrid = GameObject.Find("GameController").GetComponent<TrueGrid>();
    }

    // Maybe for showing gust of wind?
    void Update()
    {
    }

    /// <summary>
    /// Will notify all objects in its path so they can be attempted to be blown.
    /// Call this when any object is moved.
    /// </summary>
    public void NotifyObjectsInPath(GameObject ignoreGameObject)
    {
        DIRECTION direction = GetDirection(gameObject.transform.up);
        Vector2Int tileDirection = GetOffset(direction);
        Vector2Int currentTilePos = gameGrid.GetGridSpace(gameObject, false);
        Vector2Int currentCheckPos = currentTilePos + tileDirection;
        HashSet<GameObject> notifiedNeighbors = new HashSet<GameObject>();

        // Regular loop to prevent any possible issue of infinite loop with while loop
        for (int i = 0; i < 100; i++)
        {
            if (!gameGrid.PositionisWithinGrid(currentCheckPos.x, currentCheckPos.y)) return;

            List<GameObject> objectsInPath = gameGrid.GetElementsAtLocation(currentCheckPos.x, currentCheckPos.y);
            NotifyAnyMoveablesNotSelf(objectsInPath, notifiedNeighbors, ignoreGameObject);
            foreach (GameObject objectInPath in objectsInPath)
            {
                if (!objectInPath.CompareTag("notwindblocking"))
                {
                    return;
                }
            }
            currentCheckPos += tileDirection;

            // if this fires, we have a BIG problem....
            // It means the game kept checking forever in front of wind tile and never hit a wall.
            Debug.Assert(i != 99);
        }
    }
    private void NotifyAnyMoveablesNotSelf(List<GameObject> neighbors, HashSet<GameObject> notifiedNeighbors, GameObject ignoreGameObject)
    {
        //make copy so we dont get collection modified exception
        List<GameObject> modifiableNeighbors = new List<GameObject>(neighbors);
        foreach (GameObject neighbor in modifiableNeighbors)
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
    }

    /// <summary>
    /// Will push object if it is able to and object is in path of wind and unblocked
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
                foreach(GameObject objectInPath in objectsInPath)
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
