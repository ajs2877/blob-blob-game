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
    private Sprite[] sprites;

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[numberOfRequirePlayers - 1];
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            playerCount++;
        }

        if(playerCount == numberOfRequirePlayers)
        {
            LoadLevel();
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerCount--;
        }
    }

    private void LoadLevel()
    {
        Debug.Log("Level Loaded!");
        StageProgress.SetCompletedLevel(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(nextStage);
    }
}
