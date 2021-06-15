using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TrueGrid : MonoBehaviour
{
    public enum DIRECTION {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
    private Vector2Int UP = new Vector2Int(0, 1);
    private Vector2Int DOWN = new Vector2Int(0, -1);
    private Vector2Int LEFT = new Vector2Int(-1, 0);
    private Vector2Int RIGHT = new Vector2Int(1, 0);


    /// <summary>
    /// Should be how many tiles high the map is from top end to bottom end of screen.
    /// </summary>
    public int mapScale = 12;
    public Tilemap tileMap;
    public GameObject GridWallStandin;
    public GameObject MovementTargetPrefab;
    List<GameObject>[,] grid = new List<GameObject>[12,12];
    
    void Awake()
    {
        tileMap = GameObject.Find("Collideable").GetComponent<Tilemap>();

        // Sets up the grid with the right size and fills it with lists.
        Vector3Int size = tileMap.cellBounds.size;
        grid = new List<GameObject>[size.x, size.y];
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                grid[x, y] = new List<GameObject>();
            }
        }

        // Fills in the grid with solid gameobjects to represent the tilemap walls 
        // because we can't stuff in the tilemap themselves into the grid.
        foreach (Vector3Int pos in tileMap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = tileMap.CellToWorld(localPlace);
            if (tileMap.HasTile(localPlace))
            {
                // They got a script that adds them directly to grid so we dont need to do it ourselves here.
                // Just visually put them in the right spot on the stage.
                GameObject wallGridObject = Instantiate(GridWallStandin);
                Vector3 wallPosition = tileMap.GetCellCenterWorld(new Vector3Int(pos.x, pos.y, 1));
                wallPosition.z = 5; // always put the standin object behind the stage
                wallGridObject.transform.position = wallPosition;
            }
        }
    }
    
    private Vector2Int GetOffset(DIRECTION direction)
    {
        switch (direction)
        {
            case DIRECTION.UP:
                return UP;
            case DIRECTION.DOWN:
                return DOWN;
            case DIRECTION.LEFT:
                return LEFT;
            case DIRECTION.RIGHT:
                return RIGHT;
        }
        return UP;
    }
    
    public bool PositionisWithinGrid(Vector2Int position)
    {
        return PositionisWithinGrid(position.x, position.y);
    }
    
    public bool PositionisWithinGrid(int x, int y)
    {
        return (x < grid.GetLength(0) && x >= 0 && y < grid.GetLength(1) && y >= 0);
    }

    public Vector2Int GetGridCoordinate(GameObject origObject, DIRECTION direction)
    {
        Vector3 center = tileMap.cellBounds.center;
        Vector2Int directionOffset = GetOffset(direction);
        List<Vector2Int> currentPositions = GetElementLocation(origObject);
        Vector2Int newGridPosition = directionOffset + currentPositions[0];
        return newGridPosition;
    }

    public void SnapObjectToGrid(GameObject objectToSnap)
    {
        int pointCount = 0;
        Vector3Int size = tileMap.cellBounds.size;
        Vector3 center = tileMap.cellBounds.center;
        Vector3 averagedWorldPoint = new Vector3();
        List<Vector2Int> currentPositions = GetElementLocation(objectToSnap);
        if (currentPositions.Count == 0) return;

        foreach (Vector2Int position in currentPositions)
        {
            Vector3 worldPosition = tileMap.GetCellCenterWorld(new Vector3Int(position.x - (size.x / 2) + (int)center.x, position.y - (size.y / 2) + (int)center.y, 1));
            
            // Collect the points so we can average them later.
            if (pointCount == 0)
            {
                averagedWorldPoint = worldPosition;
            }
            else
            {
                averagedWorldPoint += worldPosition;
            }
            pointCount++;
        }

        // For 2x2 objects, we need to average the points to get it to move correctly
        averagedWorldPoint /= pointCount;
        averagedWorldPoint.z = objectToSnap.transform.position.z;

        GameObject movementTargetObject = Instantiate(MovementTargetPrefab);
        movementTargetObject.transform.position = averagedWorldPoint;
        PullParentToTarget targetScript = movementTargetObject.GetComponent<PullParentToTarget>();
        targetScript.gameObjectToPull = objectToSnap;
    }


    /// <summary>
    /// Returns the grid coordinate of object
    /// </summary>
    /// <param name="objectToCheck"></param>
    /// <returns>The grid coordinate of the object (bottom left coordinate for 2x2 elements)</returns>
    public Vector2Int GetGridSpace(GameObject objectToCheck, bool largeElement)
    {
        Vector3Int size = tileMap.cellBounds.size;
        Vector3 center = tileMap.cellBounds.center;

        // get bottom left coordinate for 2x2 elements
        float offset = largeElement ? 0.5f : 0;
        
        // moves the world position to cell position first to trim off decimals
        Vector3Int cellPosition = tileMap.WorldToCell(new Vector3(objectToCheck.transform.position.x - offset, objectToCheck.transform.position.y - offset, 1));

        return new Vector2Int(cellPosition.x + (size.x / 2) - (int)center.x, cellPosition.y + (size.y / 2) - (int)center.y);
    }


    /// <summary>
    /// Returns the world coordinate of a given grid space
    /// </summary>
    /// <param name="objectToSnap"></param>
    /// <returns>The world coordinate of the grid coordinate</returns>
    public Vector2 GetWorldSpace(Vector2 position)
    {
        Vector3Int size = tileMap.cellBounds.size;
        Vector3 center = tileMap.cellBounds.center;

        // moves the world position to cell position first to trim off decimals
        Vector3 worldPosition = tileMap.CellToWorld(new Vector3Int((int)position.x - (size.x / 2) + (int)center.x, (int)position.y - (size.y / 2) + (int)center.y, 1));

        return new Vector2(worldPosition.x, worldPosition.y);
    }


    /// <summary>
    /// Adds the element to the grid at specified location
    /// </summary>
    public void AddElement(GameObject objectToAdd, int x, int y)
    {
        if(PositionisWithinGrid(x, y))
        {
            grid[x, y].Add(objectToAdd);
        }
    }


    /// <summary>
    /// WIll find the x, z coordinate of a given object (if 2x2, returns all 4 positions)
    /// </summary>
    /// <returns>The coordinates of the object</returns>
    public List<Vector2Int> GetElementLocation(GameObject objectToFind)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y].Contains(objectToFind))
                {
                    positions.Add(new Vector2Int(x, y));
                }
            }
        }
        return positions;
    }

    /// <summary>
    /// Retrieves all gameobjects at a specified location
    /// </summary>
    /// <returns>list of gameobjects at spot</returns>
    public List<GameObject> GetElementsAtLocation(int x, int y)
    {
        if (PositionisWithinGrid(x, y))
        {
            return grid[x, y];
        }

        return new List<GameObject>();
    }

    /// <summary>
    /// Will remove the specified element from the grid.
    /// NOTE; you will still have to call destroy if deleting the gameobject.
    /// </summary>
    /// <returns>whether gameobject was found and removed</returns>
    public bool RemoveElement(GameObject objectToRemove)
    {
        bool wasRemoved = false;
        for(int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if(grid[x, y].RemoveAll(gridElement => gridElement == objectToRemove) != 0)
                {
                    wasRemoved = true;
                }
            }
        }
        return wasRemoved;
    }

    /// <summary>
    /// Removes all tagged GameObjects from a certain position.
    /// NOTE: you will still have to call destroy if deleting the gameobjects.
    /// </summary>
    /// <returns>removed gameobjects</returns>
    public List<GameObject> ClearAllTaggedAtSpot(string tag, int x, int y)
    {
        List<GameObject> removedObjects = new List<GameObject>();
        List<GameObject> objectsAtTile = grid[x, y];
        for(int index = objectsAtTile.Count - 1; index >= 0; index--)
        {
            if (objectsAtTile[index].tag.Equals(tag))
            {
                removedObjects.Add(objectsAtTile[index]);
                objectsAtTile.RemoveAt(index);
            }
        }
        return removedObjects;
    }

    /// <summary>
    /// Removes all GameObjects at the spot from the grid.
    /// NOTE; you will still have to call destroy if deleting the gameobjects.
    /// </summary>
    /// <returns>removed gameobjects</returns>
    public List<GameObject> ClearAllElementsAtSpot(int x, int y)
    {
        List<GameObject> removedObjects = new List<GameObject>();
        removedObjects.AddRange(grid[x, y]);
        grid[x, y].Clear();
        return removedObjects;
    }

    /// <summary>
    /// Moves the object to the spot
    /// </summary>
    /// <returns>Whether the moving was successful</returns>
    public bool MoveElement(GameObject objectToMove, bool canPushMoveables, DIRECTION direction)
    {
        Vector2Int directionOffset = GetOffset(direction);
        List<Vector2Int> currentPositions = GetElementLocation(objectToMove);
        if (currentPositions.Count == 0) return false;

        int pointCount = 0;
        Vector3 averagedWorldPoint = new Vector3();
        Vector3Int size = tileMap.cellBounds.size;

        // remove the old positions of the object and then manually add it to the new positions
        RemoveElement(objectToMove);
        foreach (Vector2Int position in currentPositions)
        {
            // move moveables over if in the desired spot
            Vector2Int newGridPosition = directionOffset + position;
            if (canPushMoveables)
            {
                // Need list copy to avoid crashing while looping over entries in case the original list gets modified at same time
                List<GameObject> objectsAtSpot = new List<GameObject>(GetElementsAtLocation(newGridPosition.x, newGridPosition.y));
                foreach (GameObject objectAtSpot in objectsAtSpot)
                {
                    if (objectAtSpot.tag.Equals("moveable") && objectToMove.GetComponent<GridObject>().size >= objectAtSpot.GetComponent<GridObject>().size)
                    {
                        // boulders cannot push boulders 
                        MoveElement(objectAtSpot, false, direction);
                    }
                }
            }
            // add current object to the new spot
            AddElement(objectToMove, newGridPosition.x, newGridPosition.y);


            // Collect the points so we can average them later.
            Vector3 center = tileMap.cellBounds.center;
            Vector3 worldPosition = tileMap.GetCellCenterWorld(new Vector3Int(newGridPosition.x - (size.x / 2) + (int)center.x, newGridPosition.y - (size.y / 2) + (int)center.y, 1));
            if (pointCount == 0)
            {
                averagedWorldPoint = worldPosition;
            }
            else
            {
                averagedWorldPoint += worldPosition;
            }
            pointCount++;
        }

        // For 2x2 objects, we need to average the points to get it to move correctly
        averagedWorldPoint /= pointCount;
        averagedWorldPoint.z = objectToMove.transform.position.z;

        // Remove the current puller for the object as we are assigning a new one
        GridObject gridObject = objectToMove.GetComponent<GridObject>();
        if (gridObject.currentMovementTargetObject != null) Destroy(gridObject.currentMovementTargetObject);

        // Creates the target that will pull the parent to the correct place
        GameObject movementTargetObject = Instantiate(MovementTargetPrefab);
        movementTargetObject.transform.position = averagedWorldPoint;
        PullParentToTarget targetScript = movementTargetObject.GetComponent<PullParentToTarget>();
        targetScript.gameObjectToPull = objectToMove;
        gridObject.currentMovementTargetObject = movementTargetObject;

        // If we are moving player, let it know it is being moved
        PlayerController component;
        if (objectToMove.TryGetComponent(out component))
        {
            component.isMoving = true;
        }

        return true;
    }

    /// <summary>
    /// Checks to see if the element can move to the specified spot.
    /// Call this before doing MoveElement to make sure things can be moved correctly.
    /// </summary>
    /// <returns>Direction can be moved in</returns>
    public bool CanMoveElement(GameObject objectToMove, bool checkCollisions, bool canPushMoveables, DIRECTION direction)
    {
        List<Vector2Int> currentPositions = GetElementLocation(objectToMove);
        if (currentPositions.Count == 0) return false;

        foreach (Vector2Int position in currentPositions)
        {
            Vector2Int newPosition = GetOffset(direction) + position;

            if (!PositionisWithinGrid(newPosition))
            {
                return false;
            }
            
            if (checkCollisions)
            {
                // skip the inner logic if we can fit.
                if (!CanElementFit(objectToMove, newPosition.x, newPosition.y))
                {
                    // checks if the moveable object can be moved in the same direction over
                    if (canPushMoveables)
                    {
                        // Need list copy to avoid crashing while looping over entries in case the original list gets modified at same time
                        List<GameObject> objectsAtSpot = new List<GameObject>(GetElementsAtLocation(newPosition.x, newPosition.y));
                        foreach (GameObject objectAtSpot in objectsAtSpot)
                        {
                            if (objectAtSpot.tag.Equals("moveable") && objectToMove.GetComponent<GridObject>().size >= objectAtSpot.GetComponent<GridObject>().size)
                            {
                                // boulders cannot push boulders 
                                if (!CanMoveElement(objectAtSpot, true, false, direction))
                                {
                                    return false;
                                }
                            }
                            else if(DoElementsCollide(objectToMove, objectAtSpot))
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        
        return true;
    }


    /// <summary>
    /// Tests to see if the element can move to the specified spot
    /// </summary>
    /// <returns>Element can fit</returns>
    public bool CanElementFit(GameObject objectToTest, int x, int y)
    {
        if (!PositionisWithinGrid(x, y)) return false;

        List<GameObject> objectsAtTile = grid[x, y];
        for (int index = objectsAtTile.Count - 1; index >= 0; index--)
        {
            GameObject objectAtTile = objectsAtTile[index];
            if (objectToTest == objectAtTile)
            {
                continue;
            }

            if (DoElementsCollide(objectToTest, objectAtTile))
            {
                return false;
            }
        }

        return true;
    }

    private bool DoElementsCollide(GameObject object1, GameObject object2)
    {
        // Checks to see if the objects has colliders that can block movement
        Collider2D collider1 = object1.GetComponent<Collider2D>();
        Collider2D collider2 = object2.GetComponent<Collider2D>();

        // check conditional checks first on the objects in case they are specially handled
        ConditionalBlocking condition1 = object1.GetComponent<ConditionalBlocking>();
        ConditionalBlocking condition2 = object2.GetComponent<ConditionalBlocking>();
        if(condition1 || condition2)
        {
            bool willBlock = false;
            if(condition1 && condition1.enabled && condition1.CanBlockObject(object2))
            {
                willBlock = true;
            }
            if (condition2 && condition2.enabled && condition2.CanBlockObject(object1))
            {
                willBlock = true;
            }

            // overrides default blocking behavior
            if (willBlock)
            {
                return true;
            }
        }

        if (collider1 && collider1.enabled && !collider1.isTrigger &&
            collider2 && collider2.enabled && !collider2.isTrigger)
        {
            // The layers can collide
            if (!Physics2D.GetIgnoreLayerCollision(object1.layer, object2.layer))
            {
                return true;
            }
        }
        return false;
    }
}
