using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MovementSwitcher : MonoBehaviour
{
    //public bool gridMovementMode = false;

    public bool multiBlobControl = false;
    private GameObject contolledSingleBlob = null;
    private GameObject blob1 = null;
    private GameObject blob2 = null;
    private GameObject bigBlob = null;
    private Tilemap grid = null;
    private TrueGrid gameGrid = null;

    public Text controlText;
    public Text movementText;

    public void Awake()
    {
        gameGrid = GameObject.Find("GameController").GetComponent<TrueGrid>();
        blob1 = GameObject.Find("BlueBlob");
        blob2 = GameObject.Find("RedBlob");
        bigBlob = GameObject.Find("PurpleBigBlob");
        grid = GameObject.Find("Tilemap").GetComponent<Tilemap>();

        contolledSingleBlob = blob1;

        if (blob2)
        {
            PlayerController blob2Controls = blob2.GetComponent<PlayerController>();
            blob2Controls.horizontalInput = "HorizontalMain";
            blob2Controls.verticalInput = "VerticalMain";
        }

        controlText.text = "E to switch to controlling both blobs at same time";
        movementText.text = "Singleblob controls:\nAWSD - " + (contolledSingleBlob == blob1 ? "Blue" : "Red") + " blob\nShift to switch blobs.";
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchBlobsControls();
        }
        if (!multiBlobControl && Input.GetKeyDown(KeyCode.LeftShift))
        {
            SwitchSelectedBlob();
        }
        if (bigBlob && Input.GetKeyDown(KeyCode.B))
        {
            ToggleCombinedBlob();
        }
    }

    public void ToggleCombinedBlob()
    {
        UpdateGridStatus(blob1, bigBlob.activeSelf);
        UpdateGridStatus(blob2, bigBlob.activeSelf);
        UpdateGridStatus(bigBlob, !bigBlob.activeSelf);
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
        contolledSingleBlob.GetComponent<PlayerController>().enabled = false; // disable deselected blob
        contolledSingleBlob = GetOtherBlob();
        contolledSingleBlob.GetComponent<PlayerController>().enabled = true; // enable new blob
        movementText.text = "Singleblob controls:\nAWSD - "+ (contolledSingleBlob == blob1 ? "Blue" : "Red")  + " blob\nShift to switch blobs.";
    }

    public void SwitchBlobsControls()
    {
        if (!blob2) return;

        multiBlobControl = !multiBlobControl;
        if (multiBlobControl)
        {
            blob1.GetComponent<PlayerController>().enabled = true;
            PlayerController blob2Controls = blob2.GetComponent<PlayerController>();
            blob2Controls.enabled = true;
            blob2Controls.horizontalInput = "HorizontalMain";
            blob2Controls.verticalInput = "VerticalMain";

            controlText.text = "E to switch to controlling\n1 blob at a time";
            movementText.text = "Multiblob controls:\nAWSD - Blue and Red blob";
        }
        else
        {
            PlayerController blob2Controls = blob2.GetComponent<PlayerController>();
            blob2Controls.horizontalInput = "HorizontalMain";
            blob2Controls.verticalInput = "VerticalMain";
            GetOtherBlob().GetComponent<PlayerController>().enabled = false;

            controlText.text = "E to switch to controlling both blobs at same time";
            movementText.text = "Singleblob controls:\nAWSD - " + (contolledSingleBlob == blob1 ? "Blue" : "Red") + " blob\nShift to switch blobs.";
        }
    }

    private GameObject GetOtherBlob()
    {
        return contolledSingleBlob == blob1 && blob2 ? blob2 : blob1;
    }
}
