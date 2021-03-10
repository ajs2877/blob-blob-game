using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTile : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private bool active = false;

    [SerializeField]
    private GameObject[] triggerObjects;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject tO in triggerObjects)
            tO.GetComponent<DoorTile>().setOpen(active);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        active = !active;
        spriteRenderer.sprite = sprites[active ? 1 : 0];    
    }
}
