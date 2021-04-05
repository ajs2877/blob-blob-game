using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour
{
    private Rigidbody2D rb;
    public float moveSpeed = 5f;
    public float magnitude = 100.0f;
    public LayerMask collideable;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
    }

    // Update is called once per frame
    void Update()
    {
        rb.freezeRotation = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if(collision.gameObject.tag == "Player")
        {
            rb.constraints = RigidbodyConstraints2D.None; // Unfreeze boulder tranformations
            Vector3 movement = (collision.transform.position - transform.position).normalized;
            if (!Physics2D.OverlapCircle(transform.position + movement, collideable))
            {
                transform.position = Vector3.MoveTowards(transform.position, transform.position + movement, moveSpeed * Time.deltaTime);
            }
        }
    }

    void OnCollisionExit2D(Collision2D collisions)
    {
        if(collisions.gameObject.tag == "Player")
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        }
    }
}
