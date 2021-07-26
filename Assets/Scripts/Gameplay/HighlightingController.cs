using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightingController : MonoBehaviour
{
    public GameObject highlightPrefab;
    private Dictionary<GameObject, List<GameObject>> highlightedTiles = new Dictionary<GameObject, List<GameObject>>();

    /**
     * Adds highlighting for this tile and all tiles attached to this triggerable/door
     */
    void OnTriggerEnter2D(Collider2D col)
    {
        // Check if we entered a triggerable tile (ignore keys)
        Triggerable triggerable = col.gameObject.GetComponent<Triggerable>();
        if (triggerable && !col.gameObject.GetComponent<KeyBehavior>())
        {
            // Grab the objects that are listening in on this triggerable
            List<GameObject> recievers = triggerable.triggerRecievers;
            foreach(GameObject reciever in recievers)
            {
                // highlight the reciever
                SpawnHighlight(col.gameObject, reciever);
                DoorTile door = reciever.GetComponent<DoorTile>();

                // highlight all the triggers and togglers attached to this reciever
                if (door)
                {
                    foreach(Triggerable trigger in door.allTriggers){
                        SpawnHighlight(col.gameObject, trigger.gameObject);
                    }
                    foreach (Triggerable toggler in door.allTogglers)
                    {
                        SpawnHighlight(col.gameObject, toggler.gameObject);
                    }
                }
            }
        }
    }

    /**
     * Remove all highlighting for this tile 
     */
    void OnTriggerExit2D(Collider2D col)
    {
        if (highlightedTiles.ContainsKey(col.gameObject))
        {
            highlightedTiles[col.gameObject].ForEach(highlight => Destroy(highlight));
        }
    }
    
    /**
     * Will add the highlighting to the target. 
     * Make sure the keyTile passed in is the tile that we are currently on.
     */
    private void SpawnHighlight(GameObject keyTile, GameObject targetToHighlight)
    {
        if (!highlightedTiles.ContainsKey(keyTile))
        {
            highlightedTiles.Add(keyTile, new List<GameObject>());
        }
        
        GameObject newHightlight = Instantiate(highlightPrefab, targetToHighlight.transform.position, Quaternion.identity);
        bool isLarge = targetToHighlight.GetComponent<GridObject>().size == 2;
        if (isLarge) newHightlight.transform.localScale *= 2;

        highlightedTiles[keyTile].Add(newHightlight);
    }
}