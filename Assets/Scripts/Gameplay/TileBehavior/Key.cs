using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Triggerable
{
    [SerializeField]
    float speed = 4f;
    [SerializeField]
    float triggerRange = .5f;

    private bool isFollowing;
    private Transform target;

    public GameObject door;


    // Update is called once per frame
    void Update()
    {
        if (isFollowing)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        if (Vector3.Distance(door.transform.position, transform.position) < triggerRange)
        {
            triggered = true;
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if(!isFollowing)
            {
                target = other.transform;

                isFollowing = true;
                
            }
        }
    }

}
