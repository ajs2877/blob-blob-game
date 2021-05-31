using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTile : Triggerable
{
    [SerializeField]
    private Sprite[] sprites;
    
    private SpriteRenderer spriteRenderer;

    private AudioSource sound;

    private bool pressed = false;
    public float timeTillDeactivation;
    private float internalTimer;
    public GameObject timerIcon;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        sound = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        if (!pressed && timeTillDeactivation > 0 && internalTimer > 0)
        {
            float rotationAngle = (Time.fixedDeltaTime / timeTillDeactivation) * -360; // negative so it goes clockwise
            timerIcon.transform.RotateAround(timerIcon.transform.position, timerIcon.transform.forward, rotationAngle);

            internalTimer -= Time.fixedDeltaTime;
            if(internalTimer <= 0)
            {
                triggered = false;
                spriteRenderer.sprite = sprites[0];
                timerIcon.SetActive(false);
            }
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (!triggered) sound.Play();
        triggered = true;
        pressed = true;
        spriteRenderer.sprite = sprites[1];
        if (timeTillDeactivation > 0)
        {
            internalTimer = timeTillDeactivation;
            timerIcon.SetActive(true);
            timerIcon.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        pressed = false;
        if (timeTillDeactivation == 0)
        {
            triggered = false;
            spriteRenderer.sprite = sprites[0];
        }
    }
}
