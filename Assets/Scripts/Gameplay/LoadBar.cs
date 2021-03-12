using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadBar : MonoBehaviour
{
    public GameObject sliderObj;
    private Slider slider;

    private float targetProgress = 1;
    public float fillSpeed = .7f;

    private void Awake()
    {
        slider = sliderObj.GetComponent<Slider>();
    }

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        // increase the load bar
        if (slider.value < targetProgress && Input.GetKey(KeyCode.R))
        {
            sliderObj.SetActive(true);
            slider.value += fillSpeed * Time.deltaTime;

            if (slider.value == 1f)
            {
                RestartProgress();
            }
        }
        // if R key is released, load bar resets to 0
        if (Input.GetKeyUp(KeyCode.R))
        {
            sliderObj.SetActive(false);
            slider.value = 0;
        }
    }

    public void RestartProgress()
    {
       SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
