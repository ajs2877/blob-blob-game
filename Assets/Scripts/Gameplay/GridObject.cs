using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    void Start()
    {
        // Adds itself to the grid and snaps to grid coordinate
        TrueGrid gameGrid = GameObject.Find("GameController").GetComponent<TrueGrid>();
        Vector2Int gridCoordinate = gameGrid.SnapObjectToGridAndAdd(gameObject);
        gameGrid.AddElement(gameObject, gridCoordinate.x, gridCoordinate.y);
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
