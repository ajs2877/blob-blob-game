using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllowOnlyBlobs : ConditionalBlocking
{
    public override bool CanBlockObject(GameObject objectToTestAgainst)
    {
        return !(objectToTestAgainst.CompareTag("Player"));
    }
}
