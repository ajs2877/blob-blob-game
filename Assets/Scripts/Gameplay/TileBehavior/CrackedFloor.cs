﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackedFloor : MonoBehaviour
{
    public GameObject pitObject;
    public GameObject LargePitObject;

    [SerializeField]
    private BoxCollider2D floorTriggerCollider;
    private TrueGrid gameGrid;
    private AudioSource sound;

    // Start is called before the first frame update
    void Start()
    {
        gameGrid = GameObject.Find("GameController").GetComponent<TrueGrid>();
        floorTriggerCollider = GetComponent<BoxCollider2D>();
        sound = GetComponentInParent<AudioSource>();
    }


    void OnTriggerExit2D(Collider2D col)
    {
        // Detects when player exits this floor
        GameObject gameObjectTouching = col.gameObject;

        if (gameObjectTouching.tag.Equals("Player"))
        {
            if (transform.parent.GetComponent<GridObject>().size == 2)
            {
                Instantiate(LargePitObject, transform.position, transform.rotation);
            }
            else
            {
                Instantiate(pitObject, transform.position, transform.rotation);
            }

            // Play the sound clip
            sound.Play();

            // remove the cracked floor from grid
            gameGrid.RemoveElement(transform.parent.gameObject);
            Destroy(transform.parent.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        // Detects when the boulder is touching
        GameObject gameObjectTouching = col.gameObject;

        if (gameObjectTouching.tag.Equals("moveable") && gameObjectTouching.name.Contains("Boulder"))
        {
            // Will check between boulder and pit to know if boulder has fallen in
            float distance = Vector3.Distance(col.transform.position, floorTriggerCollider.transform.position);
            bool isLarge = transform.parent.GetComponent<GridObject>().size == 2;
            if (distance < (isLarge ? 0.8f : 0.4f))
            {
                if(isLarge)
                {
                    Instantiate(LargePitObject, transform.position, transform.rotation);
                }
                else
                {
                    Instantiate(pitObject, transform.position, transform.rotation);
                }

                // Play the sound clip
                sound.Play();

                // remove the cracked floor from grid
                gameGrid.RemoveElement(transform.parent.gameObject);
                Destroy(transform.parent.gameObject);
            }
        }
    }
}
