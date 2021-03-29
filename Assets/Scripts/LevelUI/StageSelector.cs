using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelector : MonoBehaviour
{
    public GameObject stageHolder;
    public bool resetStageProgress = false;
    
    void Start()
    {
        if (resetStageProgress)
        {
            StageProgress.ResetProgress();
        }

        int playerProgress = StageProgress.GetCompletedLevels();
        int numOfChildren = stageHolder.transform.childCount;
        for (int index = 0; index < numOfChildren; ++index)
        {
            // Player progress starts at 1. Not 0 like index.
            if (index >= playerProgress)
            {
                // Disable all stages we have not reached yet
                stageHolder.transform.GetChild(index).GetComponent<Button>().interactable = false;
            }
        }
    }
}
