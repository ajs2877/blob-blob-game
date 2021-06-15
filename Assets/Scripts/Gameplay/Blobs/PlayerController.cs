using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public GameObject bigBlob;
    public bool isMoving = false;
    public PullParentToTarget puller = null;
    private GridObject gridObject;
    private GameObject otherBlob;
    private MovementSwitcher movementController;
    private DirectionVector directionVector;

    void Start()
    {
        gameGrid = GameObject.Find("GameController").GetComponent<TrueGrid>();
        movementController = GameObject.Find("GameController").GetComponent<MovementSwitcher>();
        gridObject = GetComponent<GridObject>();
        directionVector = GetComponent<DirectionVector>();
    }

    void Update()
    {
        // Determines if blob has truly stopped moving
        if(directionVector.direction.magnitude == 0)
        {
            isMoving = false;
        }

        // Only allow controls when we are not moving and has no puller
        if (!isMoving && puller == null && !gridObject.isSliding)
        {
            if (Input.GetAxisRaw(horizontalInput) == 1f)
            {
                MovePlayer(TrueGrid.DIRECTION.RIGHT, false, true);
            }
            else if (Input.GetAxisRaw(horizontalInput) == -1f)
            {
                MovePlayer(TrueGrid.DIRECTION.LEFT, false, true);
            }
            else if (Input.GetAxisRaw(verticalInput) == 1f)
            {
                MovePlayer(TrueGrid.DIRECTION.UP, false, true);
            }
            else if (Input.GetAxisRaw(verticalInput) == -1f)
            {
                MovePlayer(TrueGrid.DIRECTION.DOWN, false, true);
            }
        }
    }

    /// <summary>
    /// Move the player in the given direction if it is able to.
    /// </summary>
    public void MovePlayer(TrueGrid.DIRECTION directionToMove, bool cancelPreviousMovement, bool canPushStuff)
    {
        if (gameGrid.CanMoveElement(gameObject, true, true, directionToMove))
        {
            bool canMove = true;
            Vector2Int posMovingTowards = gameGrid.GetGridCoordinate(gameObject, directionToMove);

            // Have to check if it is possible for blob merging to even happen
            if (movementController.allowMerging && gridObject.size == 1 && bigBlob)
            {
                List<GameObject> listOfObjects = gameGrid.GetElementsAtLocation(posMovingTowards.x, posMovingTowards.y);
                foreach (GameObject objectAtSpot in listOfObjects)
                {
                    // Look to see if we are moving into the other player.
                    if (objectAtSpot.GetComponent<PlayerController>() != null && objectAtSpot != gameObject)
                    {
                        otherBlob = objectAtSpot;

                        // If other blob is on the exit goal tile, allow movement into it without merging
                        if (listOfObjects.Find(obj => obj.tag.Equals("exit")))
                        {
                            canMove = true;
                            break;
                        }

                        canMove = CanCombine(posMovingTowards);
                        if (canMove)
                        {
                            isMoving = true;
                            // Merge the blobs without blocking the code or thread
                            StartCoroutine(MergeBlobs(posMovingTowards));
                        }
                        break;
                    }
                }
            }

            if (canMove && AdditionalChecks(posMovingTowards))
            {
                isMoving = true;
                if (puller && cancelPreviousMovement)
                {
                    // Cancel old movement so we can force our own new target.
                    Destroy(puller.gameObject);
                }
                gameGrid.MoveElement(gameObject, canPushStuff, directionToMove);
            }
        }
    }

    /// <summary>
    /// Sees if we are moving into another player and check if merging is possible
    /// </summary>
    /// <param name="posMovingTowards">position moving to</param>
    /// <returns>can move to spot</returns>
    private bool CanCombine(Vector2Int posMovingTowards)
    {
        // Check if we have room to even combine
        for (int attemptOffsetX = 0; attemptOffsetX >= -1; attemptOffsetX--)
        {
            for (int attemptOffsetY = 0; attemptOffsetY >= -1; attemptOffsetY--)
            {
                bool roomAvaliable = true;

                // checks this 2x2 square if 2x2 blob can fit
                for (int squareX = 1 + attemptOffsetX; squareX >= 0 + attemptOffsetX; squareX--)
                {
                    for (int squareY = 1 + attemptOffsetY; squareY >= 0 + attemptOffsetY; squareY--)
                    {
                        roomAvaliable = roomAvaliable && gameGrid.CanElementFit(gameObject, posMovingTowards.x + squareX, posMovingTowards.y + squareY);
                    }
                }

                if (roomAvaliable)
                {
                    return true;
                }
            }
        }

        // no room was found
        return false;
    }

    /// <summary>
    /// Do miscellanious checks here for player movement into stuff that could be special case with blocking movement.
    /// Like a tile that blocks small blobs but not large blobs
    /// </summary>
    /// <param name="posMovingTowards">position moving to</param>
    /// <returns>can move to spot</returns>
    private bool AdditionalChecks(Vector2Int posMovingTowards)
    {
        // Prevent blobs from colliding on stages where blob merging is turned off
        if (!movementController.allowMerging)
        {
            List<GameObject> listOfObjects = gameGrid.GetElementsAtLocation(posMovingTowards.x, posMovingTowards.y);
            foreach (GameObject objectAtSpot in listOfObjects)
            {
                // Look to see if we are moving into the other player.
                if (objectAtSpot.GetComponent<PlayerController>() != null && objectAtSpot != gameObject)
                {
                    // If other blob is on the exit goal tile, allow movement into it without merging
                    if (listOfObjects.Find(obj => obj.tag.Equals("exit")))
                    {
                        return true;
                    }

                    return false;
                }
            }
        }

        return true;
    }


    private IEnumerator MergeBlobs(Vector2Int posMovingTowards)
    {
        // Delay coroutine to allow blobs time to merge
        yield return new WaitForSeconds(0.2f);

        Vector2 newSpotAvg;
        for (int attemptOffsetX = 0; attemptOffsetX >= -1; attemptOffsetX--)
        {
            for (int attemptOffsetY = 0; attemptOffsetY >= -1; attemptOffsetY--)
            {
                newSpotAvg = new Vector2();
                bool roomAvaliable = true;

                // checks this 2x2 square if 2x2 blob can fit
                // There must be a spot as this returned true before
                for (int squareX = 1 + attemptOffsetX; squareX >= 0 + attemptOffsetX; squareX--)
                {
                    for (int squareY = 1 + attemptOffsetY; squareY >= 0 + attemptOffsetY; squareY--)
                    {
                        roomAvaliable = roomAvaliable && gameGrid.CanElementFit(gameObject, posMovingTowards.x + squareX, posMovingTowards.y + squareY);
                        if (roomAvaliable)
                        {
                            newSpotAvg += new Vector2(posMovingTowards.x + squareX, posMovingTowards.y + squareY);
                        }
                    }
                }

                if (roomAvaliable)
                {
                    // get center spot
                    newSpotAvg /= 4;
                    Vector2 worldPos = gameGrid.GetWorldSpace(newSpotAvg);
                    bigBlob.transform.position = new Vector3(worldPos.x + 0.55f, worldPos.y + 0.55f, bigBlob.transform.position.z);
                    bigBlob.SetActive(true);
                    PlayerController bigBlobController = bigBlob.GetComponent<PlayerController>();
                    bigBlobController.enabled = true;
                    bigBlobController.isMoving = true;
                    bigBlob.GetComponent<GridObject>().SnapAndAddToGrid();

                    otherBlob.SetActive(false);
                    gameObject.SetActive(false);
                    gameGrid.RemoveElement(otherBlob);
                    gameGrid.RemoveElement(gameObject);

                    yield break;
                }
            }
        }
    }
}
