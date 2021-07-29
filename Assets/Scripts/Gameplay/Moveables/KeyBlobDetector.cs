using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBlobDetector : MonoBehaviour
{
    private KeyBehavior keyBehavior;

    void Start()
    {
        keyBehavior = transform.parent.GetComponent<KeyBehavior>();
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other)
    {
        keyBehavior.PickUpKey(other);
    }
}
