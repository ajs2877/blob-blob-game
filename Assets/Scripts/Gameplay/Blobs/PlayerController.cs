﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public float moveSpeed = 10f;
    [SerializeField]
    private Transform movePoint;
    [SerializeField]
    private LayerMask blockingLayerMask;
    [SerializeField]
    public string horizontalInput;
    [SerializeField]
    public string verticalInput;

    private bool hasSnapped = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float movementAmount = moveSpeed * Time.deltaTime;
        Vector3 newPos = Vector3.MoveTowards(transform.position, movePoint.position, movementAmount);
        Vector3 diff = transform.position - newPos;
        diff.z = 0;
        transform.position = newPos;
        movePoint.position += diff;

        if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
        {
            if (Mathf.Abs(Input.GetAxisRaw(horizontalInput)) == 1f)
            {
                Move(new Vector3(Input.GetAxisRaw(horizontalInput), 0, 0));
            }
            else if (Mathf.Abs(Input.GetAxisRaw(verticalInput)) == 1f)
            {
                Move(new Vector3(0, Input.GetAxisRaw(verticalInput), 0));
            }
        }

        if (!hasSnapped && 
            Input.GetAxisRaw(horizontalInput) == 0 && 
            Input.GetAxisRaw(verticalInput) == 0 && 
            Vector2.Distance(transform.position, movePoint.position) < 0.06f)
        {
            float xRemainder = transform.position.x % (0.845f) + 0.2f;
            float yRemainder = transform.position.y % (0.845f) - 0.3f;
            Vector3 gridPos = new Vector3(transform.position.x - xRemainder, transform.position.y - yRemainder, -1f);
            transform.position = gridPos;
            movePoint.position = transform.position;
        }
    }

    private void Move(Vector3 direction)
    {
        Vector3 newPosition = movePoint.position + direction * 0.845f;
        if (!Physics2D.OverlapCircle(newPosition, 0.2f, blockingLayerMask))
        {
            movePoint.position = newPosition;
            hasSnapped = false;
        }
    }
}
