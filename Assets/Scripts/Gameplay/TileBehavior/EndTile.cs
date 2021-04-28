using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTile : MonoBehaviour
{
    private int playerCount = 0;
    public int numberOfRequirePlayers = 2;
    public Utilities.SceneField nextStage;

    [SerializeField]
    private GameObject outlinePrefab;

    [SerializeField]
    private Sprite[] blobSprites;

    private List<GameObject> spawnedOutlines = new List<GameObject>();

    // Spawn outlines after grid snapping script ran
    private void Awake()
    {
        if (numberOfRequirePlayers == 1)
        {
            SpawnOutline(0.025f);
        }
        else
        {
            SpawnOutline(-0.17f);
            SpawnOutline(0.23f);
        }
    }

    private void SpawnOutline(float xOffset)
    {
        GameObject outline = Instantiate(outlinePrefab, transform.position, transform.rotation);
        outline.transform.Translate(new Vector3(xOffset, 0.575f, -2));
        outline.transform.SetParent(transform);
        spawnedOutlines.Add(outline);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerCount++;
            if (numberOfRequirePlayers == 2)
            {
                if (col.gameObject.name.Contains("Blue"))
                {
                    spawnedOutlines[0].GetComponent<SpriteRenderer>().sprite = blobSprites[1];
                }
                else
                {
                    spawnedOutlines[1].GetComponent<SpriteRenderer>().sprite = blobSprites[2];
                }
            }
        }

        if (playerCount == numberOfRequirePlayers || col.gameObject.name.Equals("PurpleBigBlob"))
        {
            LoadLevel();
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerCount--;
            if (numberOfRequirePlayers == 2)
            {
                if (col.gameObject.name.Contains("Blue"))
                {
                    spawnedOutlines[0].GetComponent<SpriteRenderer>().sprite = blobSprites[0];
                }
                else
                {
                    spawnedOutlines[1].GetComponent<SpriteRenderer>().sprite = blobSprites[0];
                }
            }
        }
    }

    private void LoadLevel()
    {
        Debug.Log("Level Loaded!");
        StageProgress.SetCompletedLevel(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(nextStage);
    }
}
