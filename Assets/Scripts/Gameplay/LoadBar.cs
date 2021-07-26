using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadBar : MonoBehaviour
{
    public GameObject sliderObj;
    private Slider slider;
    public Image deathBackground;
    public Text deathText;

    private float targetProgress = 1;
    public float fillSpeed = 1.8f;
    public bool blobKilled = false;
    private bool shouldRunUpdate = true;

    private void Awake()
    {
        slider = sliderObj.GetComponent<Slider>();
    }

    void Update()
    {
        if (shouldRunUpdate)
        {
            // increase the load bar
            if (slider.value < targetProgress && (Input.GetKey(KeyCode.R) || blobKilled))
            {
                ProgressBar(blobKilled);
            }
            // if R key is released, load bar resets to 0
            if (!blobKilled && Input.GetKeyUp(KeyCode.R))
            {
                sliderObj.SetActive(false);
                slider.value = 0;
            }
        }
    }

    public void ProgressBar(bool shouldShowDeathScreen)
    {
        sliderObj.SetActive(true);
        if (shouldShowDeathScreen)
        {
            slider.value += (fillSpeed * Time.deltaTime * 0.3f);
        }
        else
        {
            slider.value += (fillSpeed * Time.deltaTime);
        }

        if (shouldShowDeathScreen)
        {
            Color color = deathBackground.color; 
            color.a += 0.0006f;
            deathBackground.color = color;

            color = deathText.color;
            color.a += 0.0006f;
            deathText.color = color;
        }

        if (slider.value >= targetProgress)
        {
            ColorBlock seleCol = slider.colors;
            deathBackground.color = new Color(0, 0, 0, 0);
            deathText.color = new Color(255, 255, 255, 0);
            RestartProgress();
        }
    }

    public void RestartProgress()
    {
       SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
