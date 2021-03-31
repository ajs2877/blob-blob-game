using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageButton : MonoBehaviour
{
    public string stageName;

    // Update is called once per frame
    public void LoadScene()
    {
        SceneManager.LoadScene(stageName);
    }
}
