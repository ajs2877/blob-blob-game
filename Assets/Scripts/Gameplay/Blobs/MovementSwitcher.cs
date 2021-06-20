using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MovementSwitcher : MonoBehaviour
{
    //public bool gridMovementMode = false;
    
    private GameObject contolledSingleBlob = null;
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
        if(temp) blob1 = temp;

        temp = GameObject.Find("RedBlob");
        if (temp) blob2 = temp;

        temp = GameObject.Find("PurpleBigBlob");
        if (temp) bigBlob = temp;
        
        contolledSingleBlob = blob1;

        if (blob2)
        {
            PlayerController blob2Controls = blob2.GetComponent<PlayerController>();
            blob2Controls.horizontalInput = "HorizontalMain";
            blob2Controls.verticalInput = "VerticalMain";
        }
        contolledSingleBlob.GetComponent<PlayerController>().isBeingControlled = true;
        movementText.text = "Controls:\nAWSD - " + (contolledSingleBlob == blob1 ? "Blue" : "Red") + " blob\nShift to switch blobs.";
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (allowMerging && bigBlob && bigBlob.activeSelf)
            {
                // Only allow splitting if not sliding or moving
                DirectionVector directionVec = bigBlob.GetComponent<DirectionVector>();
                PlayerController playerController = bigBlob.GetComponent<PlayerController>();
                if (!directionVec.isSliding && !playerController.isMoving)
                {
                    SplitBlob();
                }
            }
            else
            {
                SwitchSelectedBlob();
            }
        }
    }

    public void ToggleCombinedBlob()
    {
        UpdateGridStatus(blob1, bigBlob.activeSelf);
        UpdateGridStatus(blob2, bigBlob.activeSelf);
        UpdateGridStatus(bigBlob, !bigBlob.activeSelf);
        movementText.text = "Controls:\nAWSD - Purple blob\nShift to switch blobs.";
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
        contolledSingleBlob.GetComponent<PlayerController>().isBeingControlled = false; // disable deselected blob
        contolledSingleBlob = GetOtherBlob();
        contolledSingleBlob.GetComponent<PlayerController>().isBeingControlled = true; // enable new blob

        movementText.text = "Controls:\nAWSD - "+ (contolledSingleBlob == blob1 ? "Blue" : "Red")  + " blob\nShift to switch blobs.";
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
        bigBlob.SetActive(false);

        movementText.text = "Controls:\nAWSD - " + (contolledSingleBlob == blob1 ? "Blue" : "Red") + " blob\nShift to switch blobs.";
    }

    private GameObject GetOtherBlob()
    {
        return contolledSingleBlob == blob1 && blob2 ? blob2 : blob1;
    }
}
