using System.Collections;
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
    private LayerMask boulderLayerMask;

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
        float movementAmount = moveSpeed * Time.deltaTime; // How far to move
        Vector3 newPos = Vector3.MoveTowards(transform.position, movePoint.position, movementAmount); // Where it moves to
        Vector3 diff = transform.position - newPos; // Vector distance moved
        diff.z = 0;
        transform.position = newPos; // Move to new position
        movePoint.position += diff; // Move movePoint

        if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f) 
        {
            if (Mathf.Abs(Input.GetAxisRaw(horizontalInput)) == 1f) 
            {
                Move(new Vector3(Input.GetAxisRaw(horizontalInput), 0, 0));
            }
            else if (Mathf.Abs(Input.GetAxisRaw(verticalInput)) == 1f)
            {
                Move(new Vector3(0, Input.GetAxisRaw(verticalInput), 0));
                Debug.Log(Input.GetAxisRaw(verticalInput));
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
        Vector3 newPosition = movePoint.position + direction * 0.845f; // Calculate new position
        
        // Check if boulder is moveable when colliding
        bool boulderCollide = !Physics2D.OverlapCircle(newPosition, 0.2f, boulderLayerMask); //False when there is a collision with a boulder
        if (!boulderCollide)
        {
            boulderCollide = Physics2D.OverlapCircle(newPosition, 0.2f, boulderLayerMask).GetComponent<BoulderGrid>().CanMove(direction); //True if boulder can move in that direction
        }

        if (!Physics2D.OverlapCircle(newPosition, 0.2f, blockingLayerMask) && boulderCollide) // Check collisions with walls
        {
            movePoint.position = newPosition;
            hasSnapped = false;
        }

        
    }
}
