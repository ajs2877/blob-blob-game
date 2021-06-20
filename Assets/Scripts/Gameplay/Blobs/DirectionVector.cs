﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionVector : MonoBehaviour
{
    public Vector2 previousDirection = new Vector2(0, 0);
    public Vector2 direction = new Vector2(0, 0);
    private Vector2 oldPos;
    public bool isSliding = false;


    void Update()
    {
        previousDirection = direction;
        direction = (Vector2)transform.position - oldPos;
        oldPos = transform.position;

        if(direction.magnitude == 0)
        {
            isSliding = false;
        }
    }
}
