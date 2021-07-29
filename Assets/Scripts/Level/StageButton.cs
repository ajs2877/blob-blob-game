using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageButton : MonoBehaviour
{
    public string stageName;

    public void LoadScene()
    {
        GameObject screenfade = GameObject.Find("ScreenFade");
        StartCoroutine(screenfade.GetComponent<ScreenFade>().FadeAndLoadScene(ScreenFade.FadeDirection.In, stageName));
    }
}
