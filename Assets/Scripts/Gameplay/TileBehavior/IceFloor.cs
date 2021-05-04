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

        if (direction && direction.direction.magnitude != 0)
        {
            Vector2 dirVec = direction.direction;

            // Allows the correct direction movement even if the blob is slightly off in direction
            // Source: http://answers.unity.com/answers/760408/view.html
            if (Mathf.Abs(dirVec.y) > Mathf.Abs(dirVec.x))
            {
                // North or south
                if (dirVec.y > 0) MoveObject(TrueGrid.DIRECTION.UP, col.gameObject);
                else MoveObject(TrueGrid.DIRECTION.DOWN, col.gameObject);
            }
            else
            {
                // East or West:
                if (dirVec.x > 0) MoveObject(TrueGrid.DIRECTION.RIGHT, col.gameObject);
                else MoveObject(TrueGrid.DIRECTION.LEFT, col.gameObject);
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
            PlayerController player = gameObject.GetComponent<PlayerController>();
            if (player.puller)
            {
                Destroy(player.puller.gameObject); // Cancel any player movement so we can force our own direction.
            }
            player.MovePlayer(directionToMove);
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
