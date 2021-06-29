using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LavaTile : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private bool cooledDown = false;

    [SerializeField]
    private BoxCollider2D parentCollider; // Set with inspector. GetComponentInParent isn't working.
    private BoxCollider2D lavaTriggerCollider;
    private SpriteRenderer spriteRenderer;
    private int size;
    private LoadBar resetBar;
    private TrueGrid gameGrid;

    private AudioSource sound;

    void Start()
    {
        lavaTriggerCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[0];
        sound = GetComponentInParent<AudioSource>();
        size = GetComponentInParent<GridObject>().size;
        resetBar = GameObject.Find("GameManager").GetComponent<LoadBar>();
        gameGrid = GameObject.Find("GameController").GetComponent<TrueGrid>();
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (cooledDown) return;

        // Detects when the blob is touching
        GameObject gameObjectTouching = col.gameObject;
        if (gameObjectTouching.CompareTag("Player"))
        {
            // Check if the blob is right on the lava tile
            float distance = Vector3.Distance(col.transform.position, lavaTriggerCollider.transform.position);
            if (distance < 0.75f * size)
            {
                // Water blob is on. Cool off lava and shrink blob
                if (!gameObjectTouching.name.Contains("Purple") && gameObjectTouching.GetComponent<GridObject>().size == 2)
                {
                    cooledDown = true;
                    spriteRenderer.sprite = sprites[1];
                    lavaTriggerCollider.enabled = false;
                    parentCollider.enabled = false;
                    List<Vector2Int> lavaPos = gameGrid.GetElementLocation(transform.parent.gameObject);
                    List<Vector2Int> playerPos = gameGrid.GetElementLocation(gameObjectTouching);
                    Vector2 commonPos = lavaPos.Intersect(playerPos).FirstOrDefault();
                    gameObjectTouching.GetComponent<PlayerController>().ShrinkBlob(commonPos);
                }

                // Non-water blob is on. Kill the blob
                else
                {
                    Destroy(gameObjectTouching);
                    resetBar.blobKilled = true;
                }
            }
        }
    }
}
