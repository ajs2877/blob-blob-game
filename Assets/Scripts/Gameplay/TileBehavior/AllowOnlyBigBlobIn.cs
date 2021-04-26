using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllowOnlyBigBlobIn : ConditionalBlocking
{
    public override bool CanBlockObject(GameObject objectTotestAgainst)
    {
        return !objectTotestAgainst.name.Equals("PurpleBigBlob");
    }
}
