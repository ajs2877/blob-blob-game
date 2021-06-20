using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullParentToTarget : MonoBehaviour
{
    public GameObject gameObjectToPull;
    public float moveSpeed = 5f;

    private AudioSource sound;

    private void Start()
    {
        PlayerController player = gameObjectToPull.GetComponent<PlayerController>();
        if (player)
        {
            player.isMoving = true;
            player.puller = this;
        }
        sound = gameObjectToPull.GetComponent<AudioSource>();
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

        // Play sound on large distances 
        // Protects against sound playing on start/reset
        if(sound && distance > 0.05f && sound.isActiveAndEnabled && !sound.isPlaying)
            sound.Play();

        if (distance <= 0.001f)
        {
            Destroy(gameObject);
        }
    }


    void OnDestroy()
    {
        // If we were moving player, let it know it is no longer being moved as this puller is now deleted
        PlayerController component;
        if (gameObjectToPull && gameObjectToPull.TryGetComponent(out component))
        {
            component.puller = null;
        }
    }
}