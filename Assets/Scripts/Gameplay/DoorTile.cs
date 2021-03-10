using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTile : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;

    private bool isOpen = false;

    private BoxCollider2D bc;
    private SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        bc = gameObject.GetComponent<BoxCollider2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        bc.isTrigger = isOpen;
        sr.sprite = sprites[isOpen ? 1 : 0];
    }

    public void setOpen(bool open)
    {
        isOpen = open;
    }
}
