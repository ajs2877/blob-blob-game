using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionVector : MonoBehaviour
{
    public Vector2 previousDirection = new Vector2(0, 0);
    public Vector2 direction = new Vector2(0, 0);
    private Vector2 oldPos;
    public enum DIRECTION
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        NONE
    }
    public static Vector2Int UP = new Vector2Int(0, 1);
    public static Vector2Int DOWN = new Vector2Int(0, -1);
    public static Vector2Int LEFT = new Vector2Int(-1, 0);
    public static Vector2Int RIGHT = new Vector2Int(1, 0);
    public static Vector2Int NONE = new Vector2Int(0, 0);


    void FixedUpdate()
    {
        previousDirection = direction;
        direction = (Vector2)transform.position - oldPos;
        oldPos = transform.position;
    }

    public static Vector2Int GetOffset(DIRECTION direction)
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


    public DIRECTION GetPreviousDirection()
    {
        return GetDirection(previousDirection);
    }

    public DIRECTION GetDirection()
    {
        return GetDirection(direction);
    }

    public static DIRECTION GetDirection(Vector2 direction)
    {
        if (direction.magnitude == 0) return DIRECTION.NONE;

        // Allows the correct direction movement even if the blob is slightly off in direction
        // Source: http://answers.unity.com/answers/760408/view.html
        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            // North or south
            if (direction.y > 0) return DIRECTION.UP;
            else return DIRECTION.DOWN;
        }
        else
        {
            // East or West:
            if (direction.x > 0) return DIRECTION.RIGHT;
            else return DIRECTION.LEFT;
        }
    }
}
