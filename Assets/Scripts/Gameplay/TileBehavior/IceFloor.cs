using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IceFloor : MonoBehaviour
{
    private AudioSource sound;
    private TrueGrid gameGrid;
    private Dictionary<GameObject, Vector2> sliders = new Dictionary<GameObject, Vector2>();
    private GameObject icyParentTile;

    // Start is called before the first frame update
    void Start()
    {
        sound = GetComponentInParent<AudioSource>();
        gameGrid = GameObject.Find("GameController").GetComponent<TrueGrid>();
        icyParentTile = gameObject.transform.parent.gameObject; // Ice tile parent of floor detector child
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        DirectionVector direction = col.gameObject.GetComponent<DirectionVector>();
        if (direction)
        {
            sliders.Add(col.gameObject, direction.direction);
            col.gameObject.GetComponent<GridObject>().slidersTilesTouching.Add(icyParentTile);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        sliders.Remove(col.gameObject);

        GridObject sliderGridObject = col.gameObject.GetComponent<GridObject>();
        sliderGridObject.slidersTilesTouching.Remove(icyParentTile);
        if(sliderGridObject.slidersTilesTouching.Count == 0)
        {
            sliderGridObject.isSliding = false;
        }
    }

    void Update()
    {
        var loopSafeSliders = sliders.ToArray();
        foreach (var slider in loopSafeSliders)
        {
            GridObject sliderGridObject = slider.Key.GetComponent<GridObject>();
            DirectionVector directionVector = slider.Key.GetComponent<DirectionVector>();
            if (!directionVector) continue;

            TrueGrid.DIRECTION originalDirection = GetDirection(slider.Value);

            // Update old direction with new current sliding direction
            if (directionVector.direction.magnitude != 0)
            {
                TrueGrid.DIRECTION sliderDirection = GetDirection(directionVector.direction);
                if (sliderDirection != originalDirection)
                {
                    // update direction only if key hasn't been removed
                    if (sliders.TryGetValue(slider.Key, out Vector2 oldVal))
                    {
                        // key exist
                        sliders[slider.Key] = directionVector.direction;
                    }
                }
            }
            // Slider stopped on tile. Make it keep sliding
            else if (sliderGridObject.currentMovementTargetObject == null)
            {
                MoveObject(originalDirection, slider.Key);
            } 
        }
    }

    private TrueGrid.DIRECTION GetDirection(Vector2 direction)
    {
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
    private void MoveObject(TrueGrid.DIRECTION directionToMove, GameObject slidingObject)
    {
        // Only size 1 objects can be moved safely with this method. 
        // 2x2 will need to handle their own movement and checks for ice due to their multiple tile behavior
        GridObject gridObject = slidingObject.GetComponent<GridObject>();
        List<Vector2Int> icePositions = gameGrid.GetElementLocation(icyParentTile); 
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
                    slidingObject.GetComponent<GridObject>().isSliding = true;
                }
                // move non-player stuff if possible and the size of the object is 1 tile
                else
                {
                    gameGrid.MoveElement(slidingObject, false, directionToMove);
                    slidingObject.GetComponent<GridObject>().isSliding = true;
                }
            }
            else
            {
                slidingObject.GetComponent<GridObject>().isSliding = false;
            }
        }
    }
}
