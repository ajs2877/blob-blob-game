using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelector : MonoBehaviour
{
    public Utilities.SceneField[] scenes;
    public GameObject stageHolder;
    public GameObject stageButtonPrefab;
    public bool resetStageProgress = false;
    
    void Start()
    {
        if (resetStageProgress)
        {
            StageProgress.ResetProgress();
        }

        bool lastLevelCompleted = true;
        for (int index = 0; index < scenes.Length; ++index)
        {
            GameObject newButton = Instantiate(stageButtonPrefab, stageHolder.transform.position, stageHolder.transform.rotation);
            // Set it into panel that holds stage buttons
            newButton.transform.parent = stageHolder.transform;

            // Sets up all buttons based on scenes assigned
            newButton.GetComponent<StageButton>().stageName = scenes[index].SceneName;
            newButton.GetComponentInChildren<Text>().text = (index + 1).ToString();
            
            if (!StageProgress.IsLevelCompleted(scenes[index].SceneName))
            {
                // Keep this stage active as we completed last stage. This new stage is avaliable.
                if (lastLevelCompleted)
                {
                    lastLevelCompleted = false;
                }
                else
                {
                    // Disable all stages we have not reached yet as last stage wasn't completed
                    newButton.GetComponent<Button>().interactable = false;
                }
            }
        }
    }
}
