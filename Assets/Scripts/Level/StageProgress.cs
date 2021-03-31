using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageProgress : MonoBehaviour
{
    public static void SetCompletedLevel(string finishedLevel)
    {
        PlayerPrefs.SetInt(finishedLevel, 1);
        PlayerPrefs.Save();
    }

    public static bool IsLevelCompleted(string level)
    {
        return PlayerPrefs.GetInt(level, 0) == 1;
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
    }
}
