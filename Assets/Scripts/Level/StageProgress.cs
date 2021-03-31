using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageProgress : MonoBehaviour
{
    public static void SetCompletedLevel(int latestFinishedLevel)
    {
        if (GetCompletedLevels() < latestFinishedLevel)
        {
            PlayerPrefs.SetInt("completedStage", latestFinishedLevel);
        }
    }

    public static int GetCompletedLevels()
    {
        return PlayerPrefs.GetInt("completedStage", 1);
    }

    public static void ResetProgress()
    {
        PlayerPrefs.SetInt("completedStage", 1);
    }
}
