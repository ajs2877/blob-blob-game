using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private void MoveObject(TrueGrid.DIRECTION directionToMove, GameObject slidingObject)
    {
        // Only size 1 objects can be moved safely with this method. 
        // 2x2 will need to handle their own movement and checks for ice due to their multiple tile behavior
        GridObject gridObject = slidingObject.GetComponent<GridObject>();
        List<Vector2Int> icePositions = gameGrid.GetElementLocation(gameObject.transform.parent.gameObject); // Ice tile parent of floor detector child
        List<Vector2Int> sliderPositions = gameGrid.GetElementLocation(slidingObject);

        // Only slide if the slider is on the ice tile in the true grid.
        // This fixes 2x2 objects sliding many tiles beyond the ice tiles.
        if (gridObject && icePositions.Any(icePosition => sliderPositions.Any(sliderPosition => sliderPosition == icePosition)))
        {
            if (gameGrid.CanMoveElement(slidingObject, true, false, directionToMove))
            {
                // Let players scripts control their own moving checks
                if (slidingObject.tag.Equals("Player"))
                {
                    PlayerController player = slidingObject.GetComponent<PlayerController>();
                    player.MovePlayer(directionToMove, true, false);
                }
                // move non-player stuff if possible and the size of the object is 1 tile
                else
                {
                    gameGrid.MoveElement(slidingObject, false, directionToMove);
                }
            }
        }
    }
}
