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

    public new void NotifyListeningTiles(bool canSlide)
    {
        if (GetComponent<GridObject>().enabled)
        {
            base.NotifyListeningTiles(canSlide);
        }
    }
}