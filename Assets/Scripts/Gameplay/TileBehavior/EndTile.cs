﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            LoadLevel();
        }
    }

    private void LoadLevel()
    {
        Debug.Log("Level Loaded!");
    }
}
