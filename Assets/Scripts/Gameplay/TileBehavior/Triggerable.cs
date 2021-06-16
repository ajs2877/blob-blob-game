using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Attach this to all tiles that a receiver can check to see if they should be activated.  
 */
public class Triggerable : MonoBehaviour
{
    public List<GameObject> triggerRecievers;

    public bool triggered = false;
}
