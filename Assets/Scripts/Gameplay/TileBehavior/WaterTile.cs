using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTile : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private bool infiniteSource;
    private bool drained = false;

    [SerializeField]
    private BoxCollider2D parentCollider; // Set with inspector. GetComponentInParent isn't working.
    private BoxCollider2D waterTriggerCollider;
    private SpriteRenderer spriteRenderer;
    private int size;
    private TrueGrid gameGrid;

    private AudioSource sound;


    void Start()
    {
        waterTriggerCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[0];
        sound = GetComponentInParent<AudioSource>();
        size = GetComponentInParent<GridObject>().size;
        gameGrid = GameObject.Find("GameController").GetComponent<TrueGrid>();
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (drained) return;

        // Detects when the small blob is touching
        GameObject gameObjectTouching = col.gameObject;
        if (gameObjectTouching.CompareTag("Player") && gameObjectTouching.GetComponent<GridObject>().size == 1)
        {
            // Check if the blob is right on the water tile
            float distance = Vector3.Distance(col.transform.position, waterTriggerCollider.transform.position);
            if (distance < 0.7f * size)
            {
                GrowPlayer(gameObjectTouching);
            }
        }
    }

    void GrowPlayer(GameObject playerBlob)
    {
        bool grownPlayer = playerBlob.GetComponent<PlayerController>().GrowBlobIfRoom();
        if (grownPlayer && !infiniteSource) {
            drained = true;
            spriteRenderer.sprite = sprites[1];
            parentCollider.enabled = false;
            waterTriggerCollider.enabled = false;
        }
        sound.Play();
    }
}
