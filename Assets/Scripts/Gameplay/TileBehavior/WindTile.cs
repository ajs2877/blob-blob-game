using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DirectionVector;

public class WindTile : MonoBehaviour
{
    protected TrueGrid gameGrid;

    void Start()
    {
        gameGrid = GameObject.Find("GameController").GetComponent<TrueGrid>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void TryWindPushingObject(GameObject slider)
    {
        // Cannot push 2x2 stuff
        if (slider.GetComponent<GridObject>().size == 2) return;

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
                        return;
                    }
                }
                currentCheckPos += tileDirection;
            }

            Moveables moveable = slider.GetComponent<Moveables>();
            if (gameGrid.CanMoveElement(slider, true, false, direction))
            {
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
            }
        }
    }
}
