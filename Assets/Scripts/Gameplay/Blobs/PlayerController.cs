using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public float moveSpeed = 10f;
    
    [SerializeField]
    public string horizontalInput;

    [SerializeField]
    public string verticalInput;
    
    private TrueGrid gameGrid;
    public bool isMoving = false;

    void Start()
    {
        gameGrid = GameObject.Find("GameController").GetComponent<TrueGrid>();
    }

    void Update()
    {
        // Only allow controls when we are not moving
        if (!isMoving)
        {
            if (Input.GetAxisRaw(horizontalInput) == 1f)
            {
                MovePlayer(TrueGrid.DIRECTION.RIGHT);
            }
            else if (Input.GetAxisRaw(horizontalInput) == -1f)
            {
                MovePlayer(TrueGrid.DIRECTION.LEFT);
            }
            else if (Input.GetAxisRaw(verticalInput) == 1f)
            {
                MovePlayer(TrueGrid.DIRECTION.UP);
            }
            else if (Input.GetAxisRaw(verticalInput) == -1f)
            {
                MovePlayer(TrueGrid.DIRECTION.DOWN);
            }
        }
    }

    /// <summary>
    /// Move the player in the given direction if it is able to.
    /// </summary>
    private void MovePlayer(TrueGrid.DIRECTION directionToMove)
    {
        if(gameGrid.CanMoveElement(gameObject, true, true, directionToMove))
        {
            gameGrid.MoveElement(gameObject, true, directionToMove);
        }
    }
}
