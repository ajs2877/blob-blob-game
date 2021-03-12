using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MovementSwitcher : MonoBehaviour
{
    public bool gridMovementMode = false;

    public GameObject blob1;
    public GameObject blob2;
    public Tilemap grid;

    public void Update()
    {
        if (Input.GetKeyDown("m"))
        {
            SwitchMode();
        }
    }

    public void SwitchMode()
    {
        gridMovementMode = !gridMovementMode;
        SetBlob(blob1);
        SetBlob(blob2);
    }

    private void SetBlob(GameObject blob)
    {
        if (blob)
        {
            blob.GetComponent<CubeMovement>().enabled = !gridMovementMode;
            blob.GetComponent<PlayerController>().enabled = gridMovementMode;
            if (gridMovementMode)
            {
                GameObject gridMovementMarker = blob.transform.GetChild(0).gameObject;
                //int xSize = grid.cellBounds.size.x;
                //int ySize = grid.cellBounds.size.y;
                float xRemainder = blob.transform.position.x % (0.845f) + 0.2f;
                float yRemainder = blob.transform.position.y % (0.845f) - 0.3f;

                Vector3 gridPos = new Vector3(blob.transform.position.x - xRemainder, blob.transform.position.y - yRemainder, -1f);
                blob.transform.position = gridPos;
                gridMovementMarker.transform.position = blob.transform.position;
            }
        }
    }
}
