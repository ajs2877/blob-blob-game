using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    public int size = 1;

    void Start()
    {
        // Adds itself to the grid and snaps to grid coordinate
        TrueGrid gameGrid = GameObject.Find("GameController").GetComponent<TrueGrid>();
        Vector2Int gridCoordinate = gameGrid.GetGridSpace(gameObject, size == 2);

        for(int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                gameGrid.AddElement(gameObject, gridCoordinate.x + x, gridCoordinate.y + y);
            }
        }
        gameGrid.SnapObjectToGrid(gameObject);
    }

    private void OnDestroy()
    {
        // remove object from grid when destroyed
        GameObject gameController = GameObject.Find("GameController");

        // Check for in case this is firing when stage is being reset or exiting to prevent logspam
        if (gameController)
        {
            TrueGrid gameGrid = gameController.GetComponent<TrueGrid>();
            gameGrid.RemoveElement(gameObject);
        }
    }
}
