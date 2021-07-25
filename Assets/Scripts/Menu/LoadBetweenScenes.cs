using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadBetweenScenes : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        GameObject stageSystem = GameObject.FindGameObjectWithTag("StageSystems");
        if (stageSystem != null)
        {
            Destroy(this.gameObject);
        }
    }
}
