using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Canvas pausedCanvas;
    public Canvas mainCanvas;
    bool pausedState = false;

    void Start()
    {
        // reset time scale whenever stage starts
        Time.timeScale = 1;
    }
    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SetPausedState(!pausedState);
        }
    }

    public void SetPausedState(bool incomingState)
    {
        pausedState = incomingState;
        mainCanvas.gameObject.SetActive(!pausedState);
        pausedCanvas.gameObject.SetActive(pausedState);
        Time.timeScale = pausedState ? 0 : 1;
    }
}
