using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderSound : MonoBehaviour
{
    private Vector3 lastPos;

    private AudioSource sound;


    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position;

        sound = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 displacement = transform.position - lastPos;
        lastPos = transform.position;

        if (displacement.magnitude < 0.05 && displacement.magnitude > 0.01f)
            sound.Play();

        Debug.Log(displacement.magnitude);
     
    }
}
