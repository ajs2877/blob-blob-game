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
    public GameObject WallColliderStandin;
    List<GameObject>[,] grid = new List<GameObject>[12,12];
    
    void Start()
    {
        grid = new List<GameObject>[mapScale, mapScale];
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                grid[x, y] = new List<GameObject>();
            }
        }

        BoundsInt bounds = tileMap.cellBounds;
        TileBase[] allTiles = tileMap.GetTilesBlock(bounds);
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    GameObject wallGridObject = Instantiate(WallColliderStandin);
                    wallGridObject.transform.position = tileMap.GetCellCenterWorld(new Vector3Int(x + bounds.xMin, y + bounds.yMin, 1));
                    AddElement(wallGridObject, x, y);
                }
            }
        }
        int t = 5;
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
    
    private bool PositionisWithinGrid(Vector2Int position)
    {
        return PositionisWithinGrid(position.x, position.y);
    }

    private bool PositionisWithinGrid(int x, int y)
    {
        return (x < grid.GetLength(0) && x >= 0 && y < grid.GetLength(1) && y >= 0);
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
                if(grid[x, y].Remove(objectToRemove))
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
    public bool MoveElement(GameObject objectToMove, bool checkCollisions, DIRECTION direction)
    {
        Vector2Int directionOffset = GetOffset(direction);
        List<Vector2Int> currentPositions = GetElementLocation(objectToMove);
        if (currentPositions.Count == 0) return false;

        RemoveElement(objectToMove);
        foreach (Vector2Int position in currentPositions)
        {
            Vector2Int newPosition = GetOffset(direction) + position;
            AddElement(objectToMove, newPosition.x, newPosition.y);
        }
        
        return true;
    }

    /// <summary>
    /// Checks to see if the element can move to the specified spot
    /// </summary>
    /// <returns>Direction can be moved in</returns>
    public bool CanMoveElement(GameObject objectToMove, bool checkCollisions, DIRECTION direction)
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
                if (!CanElementFit(objectToMove, newPosition.x, newPosition.y))
                {
                    return false;
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
            if (objectToTest == objectAtTile) continue;

            if (objectAtTile.GetComponent<Rigidbody2D>() || objectToTest.GetComponent<Rigidbody2D>())
            {
                int mask = LayerMask.GetMask(LayerMask.LayerToName(objectAtTile.layer));
                if (mask == (mask | (1 << objectToTest.layer)))
                {
                    return false;
                }
            }
        }

        return true;
    }
}
