using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadBar : MonoBehaviour
{
    public Slider slider;

    private float targetProgress = 0;
    public float fillSpeed = .7f;

    private void Awake()
    {
        slider = gameObject.GetComponent<Slider>();
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
            slider.value += fillSpeed * Time.deltaTime;

        }
        // if R key is released, load bar resets to 0
        if (Input.GetKeyUp(KeyCode.R))
        {
            slider.value = 0;
        }
    }

    public void RestartProgress(float newProgress)
    {
        targetProgress = slider.value + newProgress;
    }
}
