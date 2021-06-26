using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTile : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private bool infiniteSource;
    private bool drained = false;

    [SerializeField]
    private BoxCollider2D parentCollider; // Set with inspector. GetComponentInParent isn't working.
    private BoxCollider2D waterTriggerCollider;
    private SpriteRenderer spriteRenderer;
    private int size;
    private TrueGrid gameGrid;

    private AudioSource sound;


    void Start()
    {
        waterTriggerCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[0];
        sound = GetComponentInParent<AudioSource>();
        size = GetComponentInParent<GridObject>().size;
        gameGrid = GameObject.Find("GameController").GetComponent<TrueGrid>();
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (drained) return;

        // Detects when the small blob is touching
        GameObject gameObjectTouching = col.gameObject;
        if (gameObjectTouching.CompareTag("Player") && gameObjectTouching.GetComponent<GridObject>().size == 1)
        {
            // Check if the blob is right on the water tile
            float distance = Vector3.Distance(col.transform.position, waterTriggerCollider.transform.position);
            if (distance < 0.7f * size)
            {
                GrowPlayer(gameObjectTouching);
            }
        }
    }
    void GrowPlayer(GameObject playerBlob)
    {
        if (CanCombine(playerBlob))
        {
            playerBlob.transform.localScale *= 2;
            playerBlob.GetComponent<GridObject>().size += 1;

            Vector2Int playerPos = gameGrid.GetGridSpace(playerBlob, false);
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
                            roomAvaliable = roomAvaliable && gameGrid.CanElementFit(gameObject, playerPos.x + squareX, playerPos.y + squareY);
                            if (roomAvaliable)
                            {
                                newSpotAvg += new Vector2(playerPos.x + squareX, playerPos.y + squareY);
                            }
                        }
                    }

                    if (roomAvaliable)
                    {
                        // get center spot
                        newSpotAvg /= 4;
                        Vector2 worldPos = gameGrid.GetWorldSpace(newSpotAvg);

                        gameGrid.RemoveElement(playerBlob);

                        playerBlob.transform.position = new Vector3(worldPos.x + 0.55f, worldPos.y + 0.55f, playerBlob.transform.position.z);
                        PlayerController bigBlobController = playerBlob.GetComponent<PlayerController>();
                        bigBlobController.isMoving = true;
                        Destroy(bigBlobController.puller.gameObject);
                        bigBlobController.puller = null;
                        playerBlob.GetComponent<GridObject>().SnapAndAddToGrid();
                        playerBlob.GetComponent<PlayerController>().isMergingOrSplitting = true;


                        if (!infiniteSource)
                        {
                            drained = true;
                            spriteRenderer.sprite = sprites[1];
                            parentCollider.enabled = false;
                            waterTriggerCollider.enabled = false;
                        }

                        //break out of loops
                        attemptOffsetX = -2;
                        attemptOffsetY = -2;
                    }
                }
            }
        }
    }


    /// <summary>
    /// Sees if we are moving into another player and check if merging is possible
    /// </summary>
    /// <param name="posMovingTowards">position moving to</param>
    /// <returns>can move to spot</returns>
    private bool CanCombine(GameObject playerBlob)
    {
        Vector2Int playerPos = gameGrid.GetGridSpace(playerBlob, false);

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
                        roomAvaliable = roomAvaliable && gameGrid.CanElementFit(playerBlob, playerPos.x + squareX, playerPos.y + squareY);
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
}
