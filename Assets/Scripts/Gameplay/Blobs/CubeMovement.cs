using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    private Vector3 startPosition;
    public float moveSpeed = 5f;
    [SerializeField]
    private string horizontalInput;
    [SerializeField]
    private string verticalInput;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis(horizontalInput) != 0)
        {
            Vector3 movement = new Vector3(Input.GetAxis(horizontalInput), 0f, 0f);
            transform.position += movement * Time.deltaTime * moveSpeed;
        }
        else if (Input.GetAxis(verticalInput) != 0)
        {
            Vector3 movement = new Vector3(0f, Input.GetAxis(verticalInput), 0f);
            transform.position += movement * Time.deltaTime * moveSpeed;
        }
            
    }
}
