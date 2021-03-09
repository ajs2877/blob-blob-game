using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionVector : MonoBehaviour
{
    public Vector2 direction;
    private Vector2 oldPos;
    
    
    void Update()
    {
        direction = (Vector2)transform.position - oldPos;
        oldPos = transform.position;
    }
}
