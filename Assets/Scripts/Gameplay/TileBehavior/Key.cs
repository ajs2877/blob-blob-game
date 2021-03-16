using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Triggerable
{
    [SerializeField]
    float speed = 4f;

    bool isFollowing;
    Transform target;

    // Update is called once per frame
    void Update()
    {
        if (isFollowing)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
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
                //triggered = true;
            }
        }
    }

}
