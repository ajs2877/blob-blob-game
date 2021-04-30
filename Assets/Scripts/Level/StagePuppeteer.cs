using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StagePuppeteer : MonoBehaviour
{
    private List<Utilities.SceneField> scenes = new List<Utilities.SceneField>();


    void Awake()
    {
        // Prevent multiple instances of puppeteers
        if(GameObject.FindGameObjectsWithTag("Puppeteer").Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // This gameobject only created in stage selector scene and will persist across other scenes
        DontDestroyOnLoad(gameObject);

        // Grab all scenes
        scenes.AddRange(GameObject.Find("LevelSelectionCanvas").GetComponent<StageSelector>().scenes);

        // SOURCE: https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-activeSceneChanged.html
        SceneManager.sceneLoaded += ChangedActiveScene;
    }

    private void ChangedActiveScene(Scene next, LoadSceneMode mode)
    {
        string nextName = next.name;

        if (!nextName.Equals("LevelSelection"))
        {
            NameMatcher matcher = new NameMatcher(next);
            int nextSceneIndex = scenes.FindIndex(matcher.MatchScene);
            if (nextSceneIndex != -1)
            {
                if(nextSceneIndex + 1 == scenes.Count)
                {
                    // set end tile to load the main menu as we hit last level
                    GameObject.Find("EndTile").GetComponent<EndTile>().SetNextStage("MainMenu");
                }
                else
                {
                    // set end tile to load the scene after it
                    GameObject.Find("EndTile").GetComponent<EndTile>().SetNextStage(scenes[nextSceneIndex + 1].SceneName);
                }
            }
        }
    }

    private class NameMatcher
    {
        string toMatch;

        public NameMatcher(Scene scene)
        {
            toMatch = scene.name;
        }

        public bool MatchScene(Utilities.SceneField scene)
        {
            return scene.SceneName.Equals(toMatch);
        }
    }
}
