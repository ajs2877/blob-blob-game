using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullParentToTarget : MonoBehaviour
{
    public GameObject gameObjectToPull;
    public float moveSpeed = 5f;

    private void Start()
    {
        PlayerController player = gameObjectToPull.GetComponent<PlayerController>();
        if (player)
        {
            player.isMoving = true;
        }
    }

    void Update()
    {
        // Destroy self if the gameObject to pull was distroyed.
        if (gameObjectToPull == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move the GameObject to this target
        gameObjectToPull.transform.position = Vector3.MoveTowards(gameObjectToPull.transform.position, transform.position, moveSpeed * Time.deltaTime);

        // Remove target itself when GameObject is now right on it.
        float distance = Vector3.Distance(transform.position, gameObjectToPull.transform.position);

        // Play boulder sound on large distances 
        // Protects against sound playing on start/reset
        if(distance > 0.05f && gameObjectToPull.tag == "moveable")
            gameObjectToPull.GetComponent<AudioSource>().Play();

        if (distance <= 0.001f)
        {
            // If we were moving player, let it know it is no longer being moved
            PlayerController component;
            if (gameObjectToPull.TryGetComponent(out component))
            {
                component.isMoving = false;
            }
            Destroy(gameObject);
        }
    }
}