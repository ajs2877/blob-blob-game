using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreviceTile : MonoBehaviour
{
    private GameObject blobOccuping = null;
    private GameObject parentObject;

    [SerializeField]
    private AudioSource sound;
    [SerializeField]
    private Animator animator;

    void Start()
    {
        parentObject = gameObject.transform.parent.gameObject;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (blobOccuping == null && col.gameObject.CompareTag("Player") && col.GetComponent<GridObject>().size == 1)
        {
            blobOccuping = col.gameObject;

            blobOccuping.GetComponent<PlayerController>().isInCrevice = true;
            blobOccuping.GetComponent<PlayerController>().windPushable = false;
            blobOccuping.GetComponent<SpriteRenderer>().enabled = false;
            SpriteRenderer spriteRenderer = parentObject.GetComponent<SpriteRenderer>();

            parentObject.GetComponent<AllowOnlyBlobs>().enabled = false;
            blobOccuping.layer = LayerMask.NameToLayer("FreeMovement");

            if (blobOccuping.name.Contains("Blue"))
            {
                animator.Play("Base Layer.creviceTileBlue");
            }
            else
            {
                animator.Play("Base Layer.creviceTileRed");
            }
            sound.Play();
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject == blobOccuping)
        {
            blobOccuping.layer = LayerMask.NameToLayer("Blobs");
            blobOccuping.GetComponent<PlayerController>().isInCrevice = false;
            blobOccuping.GetComponent<PlayerController>().windPushable = true;
            blobOccuping.GetComponent<SpriteRenderer>().enabled = true;
            animator.Play("Base Layer.creviceTileEmpty");

            parentObject.GetComponent<AllowOnlyBlobs>().enabled = true;

            blobOccuping = null;
        }
    }
}
