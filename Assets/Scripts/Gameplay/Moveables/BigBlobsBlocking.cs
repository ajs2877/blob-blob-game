using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBlobsBlocking : ConditionalBlocking
{
    public override bool CanBlockObject(GameObject objectToTestAgainst)
    {
        // Prevents a big blob from being able to walk into another blob
        if (objectToTestAgainst.CompareTag("Player"))
        {
            bool incomingIsBigBlob = objectToTestAgainst.GetComponent<GridObject>().size == 2;
            bool thisIsBigBlob = gameObject.GetComponent<GridObject>().size == 2;
            return incomingIsBigBlob || thisIsBigBlob;
        }

        return false;
    }
}
