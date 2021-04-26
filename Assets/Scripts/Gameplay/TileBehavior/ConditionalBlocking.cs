using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalBlocking : MonoBehaviour
{
    /**
     * Override this in new classes to make tiles that block under certain circumstances or conditions.
     * Example: cracked wall that only allow big blob to move into it.
     */
    public virtual bool CanBlockObject(GameObject objectTotestAgainst)
    {
        return false;
    }
}
