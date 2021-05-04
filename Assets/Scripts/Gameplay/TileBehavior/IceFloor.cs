using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceFloor : MonoBehaviour
{
    private AudioSource sound;
    private TrueGrid gameGrid;

    // Start is called before the first frame update
    void Start()
    {
        sound = GetComponentInParent<AudioSource>();
        gameGrid = GameObject.Find("GameController").GetComponent<TrueGrid>();
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        DirectionVector direction = col.gameObject.GetComponent<DirectionVector>();

        if (direction)
        {
            Vector2 dirVec = direction.direction;

            if (dirVec.x > 0)
            {
                MoveObject(TrueGrid.DIRECTION.RIGHT, col.gameObject);
            }
            else if (dirVec.x < 0)
            {
                MoveObject(TrueGrid.DIRECTION.LEFT, col.gameObject);
            }
            else if (dirVec.y > 0)
            {
                MoveObject(TrueGrid.DIRECTION.UP, col.gameObject);
            }
            else if (dirVec.y < 0)
            {
                MoveObject(TrueGrid.DIRECTION.DOWN, col.gameObject);
            }
        }
    }

    /// <summary>
    /// Move the object in the previous direction if it is able to.
    /// </summary>
    private void MoveObject(TrueGrid.DIRECTION directionToMove, GameObject gameObject)
    {
        // Let players scripts control their own moving checks
        if (gameObject.tag.Equals("Player"))
        {
            gameObject.GetComponent<PlayerController>().MovePlayer(directionToMove);
        }
        // move non-player stuff if possible and the size of the object is 1 tile
        else
        {
            GridObject gridObject = gameObject.GetComponent<GridObject>();
            if (gridObject && gridObject.size == 1)
            {
                if (gameGrid.CanMoveElement(gameObject, true, false, directionToMove))
                {
                    gameGrid.MoveElement(gameObject, false, directionToMove);
                }
            }
        }
    }
}
