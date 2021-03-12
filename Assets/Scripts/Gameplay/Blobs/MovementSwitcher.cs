using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MovementSwitcher : MonoBehaviour
{
    //public bool gridMovementMode = false;

    public bool multiBlobControl = false;
    private GameObject contolledSingleBlob;
    public GameObject blob1;
    public GameObject blob2;
    public Tilemap grid;

    public Text controlText;
    public Text movementText;

    public void Awake()
    {
        contolledSingleBlob = blob1;
        PlayerController blob2Controls = blob2.GetComponent<PlayerController>();
        blob2Controls.horizontalInput = "HorizontalMain";
        blob2Controls.verticalInput = "VerticalMain";
        controlText.text = "E to switch to controlling both blobs at same time";
        movementText.text = "Singleblob controls:\nAWSD - " + (contolledSingleBlob == blob1 ? "Blue" : "Red") + " blob\nShift to switch blobs.";
    }

    public void Update()
    {
       // if (Input.GetKeyDown("f"))
       // {
       //     SwitchMovementMode();
       // }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchBlobsControls();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            SwitchSelectedBlob();
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
        multiBlobControl = !multiBlobControl;
        if (multiBlobControl)
        {
            blob1.GetComponent<PlayerController>().enabled = true;
            PlayerController blob2Controls = blob2.GetComponent<PlayerController>();
            blob2Controls.enabled = true;
            blob2Controls.horizontalInput = "HorizontalAlt";
            blob2Controls.verticalInput = "VerticalAlt";

            controlText.text = "E to switch to controlling\n1 blob at a time";
            movementText.text = "Singleblob controls:\nAWSD - " + (contolledSingleBlob == blob1 ? "Blue" : "Red") + " blob\nShift to switch blobs.";
        }
        else
        {
            PlayerController blob2Controls = blob2.GetComponent<PlayerController>();
            blob2Controls.horizontalInput = "HorizontalMain";
            blob2Controls.verticalInput = "VerticalMain";
            GetOtherBlob().GetComponent<PlayerController>().enabled = false;

            controlText.text = "E to switch to controlling both blobs at same time";
            movementText.text = "Multiblob controls:\nAWSD - Blue blob\nArrow keys - Red blob";
        }
    }

    private GameObject GetOtherBlob()
    {
        return contolledSingleBlob == blob1 ? blob2 : blob1;
    }

    /*
    public void SwitchMovementMode()
    {
        gridMovementMode = !gridMovementMode;
        SetBlob(blob1);
        SetBlob(blob2);
    }
    */
}
