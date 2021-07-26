using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBoulders : ConditionalBlocking
{
    public override bool CanBlockObject(GameObject objectToTestAgainst)
    {
        return objectToTestAgainst.layer == LayerMask.NameToLayer("Boulders");
    }
}