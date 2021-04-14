using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneToLoad : MonoBehaviour
{
    public Utilities.SceneField sceneToLoad;
    public bool resumeTimeScaling = true;

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
        Time.timeScale = resumeTimeScaling ? 1 : 0;
    }
}
