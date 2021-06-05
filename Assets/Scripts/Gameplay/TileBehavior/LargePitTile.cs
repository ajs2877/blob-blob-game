using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargePitTile : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private BoxCollider2D parentCollider; // Set with inspector. GetComponentInParent isn't working.
    private BoxCollider2D pitTriggerCollider;
    private SpriteRenderer spriteRenderer;

    public PitTile subPitDetector1;
    public PitTile subPitDetector2;
    public PitTile subPitDetector3;
    public PitTile subPitDetector4;
    public bool bigBoulderFilled;

    public GameObject subPit1FilledSprite;
    public GameObject subPit2FilledSprite;
    public GameObject subPit3FilledSprite;
    public GameObject subPit4FilledSprite;

    private AudioSource sound;

    void Start()
    {
        pitTriggerCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[0];
        sound = GetComponentInParent<AudioSource>();
        bigBoulderFilled = false;
    }

    void Update()
    {
        if (!bigBoulderFilled)
        {
            if (subPitDetector1.filled && !subPit1FilledSprite.activeSelf)
            {
                subPit1FilledSprite.SetActive(true);
            }
            if (subPitDetector2.filled && !subPit2FilledSprite.activeSelf)
            {
                subPit2FilledSprite.SetActive(true);
            }
            if (subPitDetector3.filled && !subPit3FilledSprite.activeSelf)
            {
                subPit3FilledSprite.SetActive(true);
            }
            if (subPitDetector4.filled && !subPit4FilledSprite.activeSelf)
            {
                subPit4FilledSprite.SetActive(true);
            }
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        // Detects when the boulder is touching
        GameObject gameObjectTouching = col.gameObject;

        if (!bigBoulderFilled && gameObjectTouching.tag.Equals("moveable") && gameObjectTouching.name.Contains("Boulder"))
        {
            // Will check between boulder and pit to know if boulder has fallen in
            float distance = Vector3.Distance(col.transform.position, pitTriggerCollider.transform.position);
            if (distance < 0.4f)
            {
                //Check if this a large boulder and there's room for it
                if (gameObjectTouching.GetComponent<GridObject>().size == 2 &&
                    !subPitDetector1.filled &&
                    !subPitDetector2.filled &&
                    !subPitDetector3.filled &&
                    !subPitDetector4.filled)
                {
                    subPitDetector1.FillPit();
                    subPitDetector2.FillPit();
                    subPitDetector3.FillPit();
                    subPitDetector4.FillPit();

                    // Play the sound clip
                    sound.Play();

                    // Remove boulder
                    Destroy(col.gameObject);
                    FillPit();
                }
            }
        }
    }

    public void FillPit()
    {
        // Set new texture of pit and remove collision
        spriteRenderer.sprite = sprites[1];
        parentCollider.enabled = false;
        pitTriggerCollider.enabled = false;
        bigBoulderFilled = true;
    }
}