using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyMoveable : Moveables
{
    void Update()
    {
        UpdateIsMoving();

        if (!isMoving && wasMoving)
        {
            NotifyListeningTiles(true);
        }

        UpdateWasMoving();
    }
}
