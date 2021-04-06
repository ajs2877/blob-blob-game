using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderGrid : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform movePoint;

    public LayerMask collideable;
    public LayerMask boulders;
    public LayerMask blobs;

    private Vector3 direction = new Vector3(0.0f, 0.0f, 0.0f);
    private bool moving = false;
    private const float MOVE_DISTANCE = 0.845f;

    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, movePoint.position) >= 0.05f)
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        if (moving) Move(direction);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            moving = true;
            direction = (transform.position - collision.transform.position).normalized;
            if (Mathf.Abs(direction.x) < 0.9f) direction.x = 0;
            if (Mathf.Abs(direction.y) < 0.9f) direction.y = 0;
            direction = direction * MOVE_DISTANCE;
        }
    }

    void OnCollisionExit2D(Collision2D collision) 
    {
        //if (collision.gameObject.tag == "Player")
            //moving = false;
    }

    void Move(Vector3 dir)
    {
        if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
        {
            if (!HasCollisions(movePoint.position + dir, collideable) && !HasCollisions(movePoint.position + dir, boulders) && !HasCollisions(movePoint.position + dir, blobs))
            {
                movePoint.position += dir;
                moving = false;
            }
        }
    }

    public bool CanMove(Vector3 dir)
    {
        if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
        {
            if (!HasCollisions(movePoint.position + dir, collideable) && !HasCollisions(movePoint.position + dir, boulders) && !HasCollisions(movePoint.position + dir, blobs))
            {
                return true;
            }
        }
        return false;
    }

    bool HasCollisions(Vector3 distance, LayerMask layer)
    {
        if (Physics2D.OverlapCircle(distance, .2f, layer)) return true;
        else return false;
    }
}
