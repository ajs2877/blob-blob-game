using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTile : MonoBehaviour
{
    private int playerCount = 0;
    public int nextStageNumber;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            playerCount++;
        }

        if(playerCount == 2)
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
        StageProgress.SetCompletedLevel(nextStageNumber);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("LevelSelection");
    }
}
