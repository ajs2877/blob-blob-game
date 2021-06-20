using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllowOnlyBigBlobIn : ConditionalBlocking
{
    public override bool CanBlockObject(GameObject objectToTestAgainst)
    {
        return !objectToTestAgainst.name.Equals("PurpleBigBlob");
    }
}
