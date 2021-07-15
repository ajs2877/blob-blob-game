using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveables : MonoBehaviour
{
    protected TrueGrid gameGrid;
    protected GridObject gridObject;
    public bool wasMoving = false;
    public bool isMoving;
    public bool isSliding = false;
    public bool windPushable = false;
    protected List<WindTile> windTiles = new List<WindTile>();
    public WindTile prevWindTile = null;
    public WindTile currentWindTile = null;
    protected DirectionVector directionVector;
    public PullParentToTarget puller = null;

    /**
     * Call in child class's Start method.
     */
    protected virtual void Start()
    {
        gameGrid = GameObject.Find("GameController").GetComponent<TrueGrid>();
        directionVector = GetComponent<DirectionVector>();
        gridObject = GetComponent<GridObject>();
        windTiles.AddRange(FindObjectsOfType(typeof(WindTile)) as WindTile[]);
    }


    /**
     * Call at the start of child's Update method
     */
    protected void UpdateIsMoving()
    {
        // Determines if blob has truly stopped moving. Do this first
        if (directionVector.direction.magnitude == 0 && puller == null)
        {
            isMoving = false;
            isSliding = false;
        }
    }


    /**
     * Call whereever between UpdateIsMoving and UpdateWasMoving in child's Update method
     */
    protected void NotifyListeningTiles()
    {
        // For ice tiles to work properly.
        // This will tell whatever ice tile we are on that we stopped and now the ice tile can let us know to keep moving or not.
        if (!isMoving && wasMoving)
        {
            if (windPushable)
            {
                prevWindTile = currentWindTile;
                currentWindTile = null;
                bool runPrevWindTile = false;
                foreach (WindTile windTile in windTiles)
                {
                    if(windTile == prevWindTile)
                    {
                        runPrevWindTile = true;
                    }
                    else if(!currentWindTile)
                    {
                        bool wasPushedByWind = windTile.TryWindPushingObject(gameObject);
                        if (wasPushedByWind)
                        {
                            currentWindTile = windTile;
                        }
                    }
                }

                // Delays prev wind tile so we can make prev wind tile stop pushing object if a new wind tile got control
                if (runPrevWindTile && currentWindTile == null)
                {
                    bool wasPushedByWind = prevWindTile.TryWindPushingObject(gameObject);
                    if (wasPushedByWind)
                    {
                        currentWindTile = prevWindTile;
                    }
                }
            }

            List<Vector2Int> moveablePositions = gameGrid.GetElementLocation(gameObject);
            foreach (Vector2Int pos in moveablePositions)
            {
                // make copy of list so we do not get a concurrent modification exception if elements are removed or added to the spot
                List<GameObject> objectsAtSpot = new List<GameObject>(gameGrid.GetElementsAtLocation(pos.x, pos.y));
                foreach (GameObject occupyingObject in objectsAtSpot)
                {
                    IceFloor iceTile = occupyingObject.GetComponentInChildren<IceFloor>();
                    if (iceTile) iceTile.TrySlidingObject(gameObject);
                }
            }
        }
    }

    /**
     * Call at the end of child's Update method
     */
    protected void UpdateWasMoving()
    {
        wasMoving = isMoving;
    }
}
