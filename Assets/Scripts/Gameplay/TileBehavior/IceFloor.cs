using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IceFloor : MonoBehaviour
{
    private AudioSource sound;
    private TrueGrid gameGrid;
    private GameObject icyParentTile;

    // Start is called before the first frame update
    void Start()
    {
        sound = GetComponentInParent<AudioSource>();
        gameGrid = GameObject.Find("GameController").GetComponent<TrueGrid>();
        icyParentTile = gameObject.transform.parent.gameObject; // Ice tile parent of floor detector child
    }

    internal void TrySlidingObject(GameObject slider)
    {
        GridObject sliderGridObject = slider.GetComponent<GridObject>();

        // Edge case check to not slide a bug blob when two blobs merge together
        PlayerController playerController = sliderGridObject.GetComponent<PlayerController>();
        if (playerController && playerController.isMergingOrSplitting)
        {
            return;
        }


        DirectionVector directionVector = slider.GetComponent<DirectionVector>();
        if (!directionVector)
        {
            return;
        }

        TrueGrid.DIRECTION sliderDirection = GetDirection(directionVector.previousDirection);
        // Only slide if the object is not already being pulled by something else.
        if(sliderDirection != TrueGrid.DIRECTION.NONE && slider.GetComponent<GridObject>().currentMovementTargetObject == null)
        {
            MoveObject(sliderDirection, slider, directionVector);
        }
    }

    private TrueGrid.DIRECTION GetDirection(Vector2 direction)
    {
        if (direction.magnitude == 0) return TrueGrid.DIRECTION.NONE;

        // Allows the correct direction movement even if the blob is slightly off in direction
        // Source: http://answers.unity.com/answers/760408/view.html
        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            // North or south
            if (direction.y > 0) return TrueGrid.DIRECTION.UP;
            else return TrueGrid.DIRECTION.DOWN;
        }
        else
        {
            // East or West:
            if (direction.x > 0) return TrueGrid.DIRECTION.RIGHT;
            else return TrueGrid.DIRECTION.LEFT;
        }
    }

    /// <summary>
    /// Move the object in the previous direction if it is able to.
    /// </summary>
    private void MoveObject(TrueGrid.DIRECTION directionToMove, GameObject slidingObject, DirectionVector directionVector)
    {
        // Only size 1 objects can be moved safely with this method. 
        // 2x2 will need to handle their own movement and checks for ice due to their multiple tile behavior
        GridObject gridObject = slidingObject.GetComponent<GridObject>();
        List<Vector2Int> icePositions = gameGrid.GetElementLocation(icyParentTile); 
        List<Vector2Int> sliderPositions = gameGrid.GetElementLocation(slidingObject);
        Moveables moveable = slidingObject.GetComponent<Moveables>();

        // Only slide if the slider is on the ice tile in the true grid.
        // This fixes 2x2 objects sliding many tiles beyond the ice tiles.
        if (moveable && gridObject && icePositions.Any(icePosition => sliderPositions.Any(sliderPosition => sliderPosition == icePosition)))
        {
            if (gameGrid.CanMoveElement(slidingObject, true, false, directionToMove))
            {
                // Let players scripts control their own moving checks
                if (slidingObject.tag.Equals("Player"))
                {
                    PlayerController player = slidingObject.GetComponent<PlayerController>();
                    player.MovePlayer(directionToMove, true, false);
                    moveable.isSliding = true;
                }
                // move non-player stuff if possible and the size of the object is 1 tile
                else
                {
                    gameGrid.MoveElement(slidingObject, false, directionToMove);
                    moveable.isSliding = true;
                }
            }
            else
            {
                moveable.isSliding = false;
            }
        }
    }
}
