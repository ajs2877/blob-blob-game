using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : Moveables
{
    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        UpdateIsMoving();
        NotifyOccupiedTiles();
        UpdateWasMoving();
    }
}
