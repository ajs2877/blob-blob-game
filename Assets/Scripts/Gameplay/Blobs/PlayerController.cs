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
    public GameObject bigBlob;
    public bool isMoving = false;
    private GridObject gridObject;
    private GameObject otherBlob;

    void Start()
    {
        gameGrid = GameObject.Find("GameController").GetComponent<TrueGrid>();
        gridObject = GetComponent<GridObject>();
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
            bool canMove = true;
            Vector2Int posMovingTowards = gameGrid.GetGridCoordinate(gameObject, directionToMove);

            // Have to check if it is possible for blob merging to even happen
            if (gridObject.size == 1 && bigBlob)
            {
                List<GameObject> listOfObjects = gameGrid.GetElementsAtLocation(posMovingTowards.x, posMovingTowards.y);
                foreach (GameObject objectAtSpot in listOfObjects)
                {
                    // Look to see if we are moving into the other player.
                    if (objectAtSpot.GetComponent<PlayerController>() != null && objectAtSpot != gameObject)
                    {
                        otherBlob = objectAtSpot;
                        canMove = CanCombine(posMovingTowards);
                        if (canMove)
                        {
                            // Merge the blobs without blocking the code or thread
                            StartCoroutine(MergeBlobs(posMovingTowards));
                        }
                        break;
                    }
                }
            }
            
            if (canMove && AdditionalChecks(posMovingTowards))
            {
                gameGrid.MoveElement(gameObject, true, directionToMove);
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
                    bigBlob.transform.position = new Vector3(worldPos.x + 0.5f, worldPos.y + 0.5f, bigBlob.transform.position.z);
                    bigBlob.SetActive(true);
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
