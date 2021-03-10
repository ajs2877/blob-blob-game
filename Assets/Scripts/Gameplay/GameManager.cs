using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float restartDelay = 1f;
    public LoadBar restartBar;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Restart Method 1
        /*if (Input.GetKeyDown(KeyCode.R))
        {
            // restarts after the amount of seconds the restartDelay variable is set to
            Invoke("Restart", restartDelay);
        }*/

        //Restart Method 2
        // Restarts the game when the load bar reaches the end
        if (Input.GetKey(KeyCode.R))
        {
            restartBar.RestartProgress(1f);

            if (restartBar.slider.value == 1f)
            {
                Restart();
            }
            
        }
        
    }

    // restarts the game by reloading the current scene
    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
