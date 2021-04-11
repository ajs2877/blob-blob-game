using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform movePoint;

    public LayerMask collideable;
    public LayerMask boulders;

    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if(Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
        {
            if (Mathf.Abs(Input.GetAxisRaw("HorizontalMain")) == 1) //Am I moving to left or right?
            {
                Vector3 distance = new Vector3(Input.GetAxisRaw("HorizontalMain"), 0f, 0f);
                if (!HasCollisions(movePoint.position + distance, collideable))
                {
                    movePoint.position += distance;
                }
            }

            if (Mathf.Abs(Input.GetAxisRaw("VerticalMain")) == 1) //Am I moving to up or down?
            {
                Vector3 distance = new Vector3(0f, Input.GetAxisRaw("VerticalMain"), 0f);
                if (!HasCollisions(movePoint.position + distance, collideable))
                {
                    movePoint.position += distance;
                }
            }
        }
        */
    }

    bool HasCollisions(Vector3 distance, LayerMask layer)
    {
        if (Physics2D.OverlapCircle(distance, .2f, layer)) return true;
        else return false;
    }
}
