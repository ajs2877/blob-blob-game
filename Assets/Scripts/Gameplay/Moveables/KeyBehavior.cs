using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBehavior : Triggerable
{
    [SerializeField]
    float speed = 4f;
    [SerializeField]
    float triggerRange = .65f;

    private bool isFollowing;
    private Transform target;

    public GameObject door;

    private AudioSource[] sounds;
    private AudioSource pickUpSound;
    private AudioSource unlockDoorSound;
    private TrueGrid gameGrid;

    void Start()
    {
        gameGrid = GameObject.Find("GameController").GetComponent<TrueGrid>();
        sounds = GetComponents<AudioSource>();
        pickUpSound = sounds[0];
        unlockDoorSound = sounds[1];
    }

    // Update is called once per frame
    void Update()
    {
        if (!transform || !target) return;

        if (isFollowing)
        {
            // make key continue to follow blobs between blob merging/splitting
            if (!target.gameObject.activeSelf)
            {
                GameObject blueBlob = GameObject.Find("BlueBlob");
                GameObject bigBlob = GameObject.Find("PurpleBigBlob");
                if (blueBlob && blueBlob.activeSelf)
                {
                    target = blueBlob.transform;
                }
                else if (bigBlob && bigBlob.activeSelf)
                {
                    target = bigBlob.transform;
                }
            }

            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        if (Vector3.Distance(door.transform.position, transform.position) < triggerRange)
        {
            if(!triggered) unlockDoorSound.Play();
            triggered = true;
            Destroy(gameObject);
        }
    }

    public void PickUpKey(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if(!isFollowing)
            {
                pickUpSound.enabled = true;
                pickUpSound.Play();
                target = other.transform;
                isFollowing = true;

                // cannot be pushed by wind when picked up by blob
                GetComponent<KeyMoveable>().windPushable = false;

                // remove it from grid
                GetComponent<GridObject>().enabled = false;
                gameGrid.RemoveElement(gameObject);
            }
        }
    }

}
