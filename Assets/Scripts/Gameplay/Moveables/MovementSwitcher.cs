using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MovementSwitcher : MonoBehaviour
{
    //public bool gridMovementMode = false;
    
    private GameObject controlledSingleBlob = null;
    public GameObject blob1 = null;
    public GameObject blob2 = null;
    public GameObject bigBlob = null;
    private TrueGrid gameGrid = null;

    public bool allowMerging = false;
    public Text movementText;

    public void Awake()
    {
        gameGrid = GameObject.Find("GameController").GetComponent<TrueGrid>();

        GameObject temp = GameObject.Find("BlueBlob");
        if (temp)
        {
            blob1 = temp;
            controlledSingleBlob = blob1;
        }

        temp = GameObject.Find("RedBlob");
        if (temp)
        {
            blob2 = temp;
            if(!blob1.activeSelf) controlledSingleBlob = blob2;
        }

        temp = GameObject.Find("PurpleBigBlob");
        if (temp) {
            bigBlob = temp;
        }
        
        if (blob2)
        {
            PlayerController blob2Controls = blob2.GetComponent<PlayerController>();
            blob2Controls.horizontalInput = "HorizontalMain";
            blob2Controls.verticalInput = "VerticalMain";
        }
        PlayerController playerController = controlledSingleBlob.GetComponent<PlayerController>();
        playerController.isBeingControlled = true;
        if (blob1.activeSelf && blob2 && blob2.activeSelf) playerController.indicator.GetComponent<SpriteRenderer>().enabled = true;
        movementText.text = "Controls:\nAWSD - " + (controlledSingleBlob == blob1 ? "Blue" : "Red") + " blob\nShift to switch blobs.";
    }

    public void Update()
    {
        // Check to make sure no blob was killed before allowing user controls
        if (bigBlob && blob1 && blob2)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (allowMerging && bigBlob && bigBlob.activeSelf)
                {
                    // Only allow splitting if not sliding or moving
                    PlayerController playerController = bigBlob.GetComponent<PlayerController>();
                    if (!playerController.isSliding && !playerController.isMoving)
                    {
                        SplitBlob();
                    }
                }
                else
                {
                    SwitchSelectedBlob();
                }
            }

            if (bigBlob.GetComponent<PlayerController>().isBeingControlled)
            {
                movementText.text = "Controls:\nAWSD - Purple blob\nShift to split up.";
            }
        }
    }

    public void UpdateGridStatus(GameObject gridObject, bool shouldExistOnGrid)
    {
        if (!gridObject) return;

        gridObject.SetActive(shouldExistOnGrid);
        if (shouldExistOnGrid)
        {
            int objectSize = gridObject.GetComponent<GridObject>().size;
            Vector2Int gridCoordinate = gameGrid.GetGridSpace(gridObject, objectSize == 2);

            for (int x = 0; x < objectSize; x++)
            {
                for (int y = 0; y < objectSize; y++)
                {
                    gameGrid.AddElement(gridObject, gridCoordinate.x + x, gridCoordinate.y + y);
                }
            }
            gameGrid.SnapObjectToGrid(gridObject);
        }
        else
        {
            gameGrid.RemoveElement(gridObject);
        }
    }

    public void SwitchSelectedBlob()
    {
        if (GetOtherBlob().activeSelf)
        {
            PlayerController playerController;

            playerController = controlledSingleBlob.GetComponent<PlayerController>();
            playerController.isBeingControlled = false; // disable deselected blob
            playerController.indicator.GetComponent<SpriteRenderer>().enabled = false;

            controlledSingleBlob = GetOtherBlob();

            playerController = controlledSingleBlob.GetComponent<PlayerController>();
            playerController.isBeingControlled = true; // enable new blob
            playerController.indicator.GetComponent<SpriteRenderer>().enabled = true;

            movementText.text = "Controls:\nAWSD - " + (controlledSingleBlob == blob1 ? "Blue" : "Red") + " blob\nShift to switch blobs.";
        }
    }

    private void SplitBlob()
    {
        Vector3 position = bigBlob.transform.position;
        blob1.transform.position = new Vector3(position.x - 0.35f, position.y - 0.1f, blob1.transform.position.z);
        blob2.transform.position = new Vector3(position.x + 0.35f, position.y - 0.1f, blob2.transform.position.z);
        blob1.SetActive(true);
        blob2.SetActive(true);
        blob1.GetComponent<GridObject>().SnapAndAddToGrid();
        blob2.GetComponent<GridObject>().SnapAndAddToGrid();

        gameGrid.RemoveElement(bigBlob);
        bigBlob.GetComponent<PlayerController>().isBeingControlled = false;
        bigBlob.SetActive(false);

        blob1.GetComponent<PlayerController>().isChangingSize = true;
        blob2.GetComponent<PlayerController>().isChangingSize = true;

        controlledSingleBlob.GetComponent<PlayerController>().indicator.GetComponent<SpriteRenderer>().enabled = true;
        movementText.text = "Controls:\nAWSD - " + (controlledSingleBlob == blob1 ? "Blue" : "Red") + " blob\nShift to switch blobs.";
    }

    private GameObject GetOtherBlob()
    {
        return controlledSingleBlob == blob1 && blob2 ? blob2 : blob1;
    }
}
